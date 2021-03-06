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
using DotNetWorkQueue.Configuration;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace DotNetWorkQueue.Tests.Configuration
{
    public class TaskSchedulerConfigurationTests
    {
        [Fact]
        public void Test_DefaultNotReadOnly()
        {
            var configuration = GetConfiguration();
            Assert.False(configuration.IsReadOnly);
        }
        [Fact]
        public void Set_Readonly()
        {
            var configuration = GetConfiguration();
            configuration.SetReadOnly();
            Assert.True(configuration.IsReadOnly);
        }
        [Theory, AutoData]
        public void SetAndGet_MaxQueueSize(int value)
        {
            var configuration = GetConfiguration();
            configuration.MaxQueueSize = value;
            Assert.Equal(value, configuration.MaxQueueSize);
        }
        [Theory, AutoData]
        public void SetAndGet_MaximumThreads(int value)
        {
            var configuration = GetConfiguration();
            configuration.MaximumThreads = value;

            Assert.Equal(value, configuration.MaximumThreads);
        }
        [Theory, AutoData]
        public void SetAndGet_WaitForThreadPoolToFinish(TimeSpan value)
        {
            var configuration = GetConfiguration();
            configuration.WaitForThreadPoolToFinish = value;

            Assert.Equal(value, configuration.WaitForThreadPoolToFinish);
        }

        [Theory, AutoData]
        public void Set_MaximumThreads_WhenReadOnly_Fails(int value)
        {
            var configuration = GetConfiguration();
            configuration.SetReadOnly();

            Assert.Throws<InvalidOperationException>(
              delegate
              {
                  configuration.MaximumThreads = value;
              });
        }
        [Theory, AutoData]
        public void Set_MaxQueueSize_WhenReadOnly_Fails(int value)
        {
            var configuration = GetConfiguration();
            configuration.SetReadOnly();

            Assert.Throws<InvalidOperationException>(
              delegate
              {
                  configuration.MaxQueueSize = value;
              });
        }
        [Theory, AutoData]
        public void Set_WaitForThreadPoolToFinish_WhenReadOnly_Fails(TimeSpan value)
        {
            var configuration = GetConfiguration();
            configuration.SetReadOnly();
            Assert.Throws<InvalidOperationException>(
              delegate
              {
                  configuration.WaitForThreadPoolToFinish = value;
              });
        }
        private TaskSchedulerConfiguration GetConfiguration()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            return fixture.Create<TaskSchedulerConfiguration>();
        }
    }
}
