// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStoreConnections"/>.
    /// </summary>
    public class EventStoreConnections : IEventStoreConnections
    {
        /// <inheritdoc />
        public DatabaseConnection GetFor(EventStoreConfiguration configuration)
            => new (new EventStoreDatabaseConfiguration{ Instance = configuration });
    }
}