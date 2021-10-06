// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.V6.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanMigrateAnEventStore" /> that can migrate from major version 6 to major version 7.
    /// /// </summary>
    public class Migrator : ICanMigrateAnEventStore
    {
        readonly IEventStoreConnections _eventStoreConnections;

        public Migrator(IEventStoreConnections eventStoreConnections)
        {
            _eventStoreConnections = eventStoreConnections;
        }

        /// <inheritdoc />
        public async Task<Try> Migrate(EventStoreConfiguration configuration)
        {
            var connection = _eventStoreConnections.GetFor(configuration);
            return new NotImplementedException();
        }
    }
}
