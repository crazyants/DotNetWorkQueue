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

using DotNetWorkQueue.IntegrationTests.Shared;
using DotNetWorkQueue.IntegrationTests.Shared.JobScheduler;
using DotNetWorkQueue.Transport.SQLite.Integration.Tests;
using DotNetWorkQueue.Transport.SQLite.Microsoft.Basic;
using DotNetWorkQueue.Transport.SQLite.Microsoft.Integration.Tests;
using DotNetWorkQueue.Transport.SQLite.Shared.Basic;
using Xunit;

namespace DotNetWorkQueue.Transport.SQLite.Linq.Microsoft.Integration.Tests.JobScheduler
{
    [Collection("SQLite")]
    public class JobSchedulerMultipleTests
    {
        [Theory]
        [InlineData(10, false),
         InlineData(10, true)]
        public void Run(
            int producerCount,
            bool inMemoryDb)
        {
            using (var connectionInfo = new IntegrationConnectionInfo(inMemoryDb))
            {
                var queueName = GenerateQueueName.Create();
                using (var queueContainer = new QueueContainer<SqLiteMessageQueueInit>(x => {
                }))
                {
                    try
                    {
                        var tests = new JobSchedulerTestsShared();
                        tests.RunTestMultipleProducers<SqLiteMessageQueueInit, SqliteJobQueueCreation>(queueName,
                            connectionInfo.ConnectionString, true, producerCount, queueContainer.CreateTimeSync(connectionInfo.ConnectionString), LoggerShared.Create(queueName, GetType().Name));
                    }
                    finally
                    {

                        using (var queueCreator =
                            new QueueCreationContainer<SqLiteMessageQueueInit>())
                        {
                            using (
                                var oCreation =
                                    queueCreator.GetQueueCreation<SqLiteMessageQueueCreation>(queueName,
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
    }
}
