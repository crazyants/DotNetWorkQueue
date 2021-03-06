﻿// ---------------------------------------------------------------------
//This file is part of DotNetWorkQueue
//Copyright © 2017 Brian Lehnen
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// ---------------------------------------------------------------------

using System;
using DotNetWorkQueue.IntegrationTests.Shared;
using DotNetWorkQueue.IntegrationTests.Shared.ConsumerMethodAsync;
using DotNetWorkQueue.IntegrationTests.Shared.ProducerMethod;
using DotNetWorkQueue.Transport.Redis.Basic;
using DotNetWorkQueue.Transport.Redis.IntegrationTests;
using Xunit;

namespace DotNetWorkQueue.Transport.Redis.Linq.Integration.Tests.ConsumerMethodAsync
{
    [Collection("Redis")]
    public class ConsumerMethodAsyncErrorTable
    {
        [Theory]
        [InlineData(1, 30, 1, 1, 0, ConnectionInfoTypes.Windows, LinqMethodTypes.Compiled),
#if NETFULL
         InlineData(1, 30, 1, 1, 0, ConnectionInfoTypes.Windows, LinqMethodTypes.Dynamic),
         InlineData(1, 30, 1, 1, 0, ConnectionInfoTypes.Linux, LinqMethodTypes.Dynamic),
#endif
        InlineData(1, 30, 1, 1, 0, ConnectionInfoTypes.Linux, LinqMethodTypes.Compiled)]
        public void Run(int messageCount, int timeOut, int workerCount, int readerCount, int queueSize, ConnectionInfoTypes type, LinqMethodTypes linqMethodTypes)
        {
            var queueName = GenerateQueueName.Create();
            var logProvider = LoggerShared.Create(queueName, GetType().Name);
            var connectionString = new ConnectionInfo(type).ConnectionString;
            using (
                var queueCreator =
                    new QueueCreationContainer<RedisQueueInit>(
                        serviceRegister => serviceRegister.Register(() => logProvider, LifeStyles.Singleton)))
            {
                try
                {
                    //create data
                    var id = Guid.NewGuid();
                    var producer = new ProducerMethodShared();
                    if (linqMethodTypes == LinqMethodTypes.Compiled)
                    {
                        producer.RunTestCompiled<RedisQueueInit>(queueName,
                            connectionString, false, messageCount, logProvider, Helpers.GenerateData,
                            Helpers.Verify, false, id, GenerateMethod.CreateErrorCompiled, 0, null);
                    }
#if NETFULL
                    else
                    {
                        producer.RunTestDynamic<RedisQueueInit>(queueName,
                            connectionString, false, messageCount, logProvider, Helpers.GenerateData,
                            Helpers.Verify, false, id, GenerateMethod.CreateErrorDynamic, 0, null);
                    }
#endif
                    //process data
                    var consumer = new ConsumerMethodAsyncErrorShared();
                    consumer.RunConsumer<RedisQueueInit>(queueName, connectionString, false,
                        logProvider,
                        messageCount, workerCount, timeOut, queueSize, readerCount, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(12), id, "second(*%3)");
                    ValidateErrorCounts(queueName, messageCount, connectionString);
                    using (
                        var count = new VerifyQueueRecordCount(queueName, connectionString))
                    {
                        count.Verify(messageCount, false, 2);
                    }

                }
                finally
                {
                    using (
                        var oCreation =
                            queueCreator.GetQueueCreation<RedisQueueCreation>(queueName,
                                connectionString)
                        )
                    {
                        oCreation.RemoveQueue();
                    }
                }
            }
        }

        private void ValidateErrorCounts(string queueName, int messageCount, string connectionString)
        {
            using (var error = new VerifyErrorCounts(queueName, connectionString))
            {
                error.Verify(messageCount, 2);
            }
        }
    }
}
