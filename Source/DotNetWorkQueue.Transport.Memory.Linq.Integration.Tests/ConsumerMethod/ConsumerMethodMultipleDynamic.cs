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

#if NETFULL
using System;
using DotNetWorkQueue.IntegrationTests.Shared;
using DotNetWorkQueue.IntegrationTests.Shared.ConsumerMethod;
using DotNetWorkQueue.IntegrationTests.Shared.ProducerMethod;
using DotNetWorkQueue.Transport.Memory.Basic;
using Xunit;
#endif

namespace DotNetWorkQueue.Transport.Memory.Linq.Integration.Tests.ConsumerMethod
{
#if NETFULL
    public class ConsumerMethodMultipleDynamic
    {
        [Theory]
        [InlineData(1000, 0, 60, 5)]
        public void Run(int messageCount, int runtime,
            int timeOut, int workerCount)
        {
            using (var connectionInfo = new IntegrationConnectionInfo())
            {
                var queueName = GenerateQueueName.Create();
                var logProvider = LoggerShared.Create(queueName, GetType().Name);
                using (
                    var queueCreator =
                        new QueueCreationContainer<MemoryMessageQueueInit>(
                            serviceRegister => serviceRegister.Register(() => logProvider, LifeStyles.Singleton)))
                {
                    try
                    {
                        using (
                            var oCreation =
                                queueCreator.GetQueueCreation<MessageQueueCreation>(queueName,
                                    connectionInfo.ConnectionString)
                            )
                        {
                            var result = oCreation.CreateQueue();
                            Assert.True(result.Success, result.ErrorMessage);

                            var producer = new ProducerMethodMultipleDynamicShared();
                            var id = Guid.NewGuid();
                            producer.RunTestDynamic<MemoryMessageQueueInit>(queueName,
                                connectionInfo.ConnectionString, false, messageCount, logProvider,
                                Helpers.GenerateData,
                                Helpers.Verify, false, id, GenerateMethod.CreateMultipleDynamic, runtime, oCreation.Scope);

                            var consumer = new ConsumerMethodShared();
                            consumer.RunConsumer<MemoryMessageQueueInit>(queueName, connectionInfo.ConnectionString,
                                false,
                                logProvider,
                                runtime, messageCount,
                                workerCount, timeOut,
                                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(35), id, "second(*%10)");

                            new VerifyQueueRecordCount()
                                .Verify(oCreation.Scope, 0, true);
                        }
                    }
                    finally
                    {
                        using (
                            var oCreation =
                                queueCreator.GetQueueCreation<MessageQueueCreation>(queueName,
                                    connectionInfo.ConnectionString)
                            )
                        {
                            oCreation.RemoveQueue();
                        }
                    }
                }
            }
        }
    }
#endif
}
