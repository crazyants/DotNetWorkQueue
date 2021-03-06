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

using System.Diagnostics.CodeAnalysis;
using DotNetWorkQueue.Transport.RelationalDatabase.Basic.Query;
using DotNetWorkQueue.Validation;

namespace DotNetWorkQueue.Transport.RelationalDatabase.Basic.QueryHandler
{
    /// <inheritdoc />
    /// <summary>
    /// Returns number of records in error queue
    /// </summary>
    internal class GetErrorCountQueryHandler : IQueryHandler<GetErrorCountQuery, long>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IPrepareQueryHandler<GetErrorCountQuery, long> _prepareQuery;
        private readonly IReadColumn _readColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetErrorCountQueryHandler" /> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="prepareQuery">The prepare query.</param>
        /// <param name="readColumn">The read column.</param>
        public GetErrorCountQueryHandler(
            IDbConnectionFactory connectionFactory,
            IPrepareQueryHandler<GetErrorCountQuery, long> prepareQuery,
            IReadColumn readColumn)
        {
            Guard.NotNull(() => connectionFactory, connectionFactory);
            Guard.NotNull(() => prepareQuery, prepareQuery);
            Guard.NotNull(() => readColumn, readColumn);
            _connectionFactory = connectionFactory;
            _prepareQuery = prepareQuery;
            _readColumn = readColumn;
        }
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Query OK")]
        public long Handle(GetErrorCountQuery query)
        {
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    _prepareQuery.Handle(query, command, CommandStringTypes.GetErrorCount);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return _readColumn.ReadAsInt64(CommandStringTypes.GetErrorCount, 0, reader);
                        }
                    }
                }
            }
            return 0;
        }
    }
}
