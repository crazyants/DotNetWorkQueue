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

namespace DotNetWorkQueue.Logging
{
    /// <inheritdoc />
    /// <summary>
    /// A NoOp logger
    /// </summary>
    public class NullLoggerProvider : ILogProvider
    {
        /// <summary>
        /// Gets the specified named logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>
        /// The logger reference.
        /// </returns>
        public Logger GetLogger(string name)
        {
            return (logLevel, messageFunc, exception, formatParameters) => true;
        }

        /// <summary>
        /// Opens a nested diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="message">The message to add to the diagnostics context.</param>
        /// <returns>
        /// A disposable that when disposed removes the message from the context.
        /// </returns>
        public IDisposable OpenNestedContext(string message)
        {
            return NullDisposable.Instance;
        }

        /// <summary>
        /// Opens a mapped diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <returns>
        /// A disposable that when disposed removes the map from the context.
        /// </returns>
        public IDisposable OpenMappedContext(string key, string value)
        {
            return NullDisposable.Instance;
        }

        /// <summary>
        /// An implementation of a NoOp class that is <see cref="IDisposable"/>
        /// </summary>
        private class NullDisposable : IDisposable
        {
            internal static readonly IDisposable Instance = new NullDisposable();

            public void Dispose()
            { }
        }
    }
}
