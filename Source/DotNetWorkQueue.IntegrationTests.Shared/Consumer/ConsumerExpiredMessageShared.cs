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
using System.Threading;
using DotNetWorkQueue.Logging;
using Xunit;

namespace DotNetWorkQueue.IntegrationTests.Shared.Consumer
{
    public class ConsumerExpiredMessageShared<TMessage>
         where TMessage : class
    {
        public void RunConsumer<TTransportInit>(string queueName, string connectionString, bool addInterceptors,
            ILogProvider logProvider,
            int runTime, int messageCount,
            int workerCount, int timeOut,
            TimeSpan heartBeatTime, TimeSpan heartBeatMonitorTime, string updateTime,
            string route)
            where TTransportInit : ITransportInit, new()
        {

            using (var metrics = new Metrics.Metrics(queueName))
            {
                var addInterceptorConsumer = InterceptorAdding.No;
                if (addInterceptors)
                {
                    addInterceptorConsumer = InterceptorAdding.ConfigurationOnly;
                }
                var processedCount = new IncrementWrapper();
                using (
                    var creator = SharedSetup.CreateCreator<TTransportInit>(addInterceptorConsumer, logProvider, metrics)
                    )
                {
                    using (
                        var queue =
                            creator.CreateConsumer(queueName,
                                connectionString))
                    {
                        SharedSetup.SetupDefaultConsumerQueue(queue.Configuration, workerCount, heartBeatTime,
                            heartBeatMonitorTime, updateTime, route);
                        queue.Configuration.MessageExpiration.Enabled = true;
                        queue.Configuration.MessageExpiration.MonitorTime = TimeSpan.FromSeconds(8);
                        var waitForFinish = new ManualResetEventSlim(false);
                        waitForFinish.Reset();
                        //start looking for work
                        queue.Start<TMessage>((message, notifications) =>
                        {
                            MessageHandlingShared.HandleFakeMessages<TMessage>(null, runTime, processedCount, messageCount,
                                waitForFinish);
                        });

                        for (var i = 0; i < timeOut; i++)
                        {
                            if (VerifyMetrics.GetExpiredMessageCount(metrics.GetCurrentMetrics()) == messageCount)
                            {
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    Assert.Equal(0, processedCount.ProcessedCount);
                    VerifyMetrics.VerifyProcessedCount(queueName, metrics.GetCurrentMetrics(), 0);
                    VerifyMetrics.VerifyExpiredMessageCount(queueName, metrics.GetCurrentMetrics(), messageCount);
                    LoggerShared.CheckForErrors(queueName);
                }
            }
        }
    }
}
