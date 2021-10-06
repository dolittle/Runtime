// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Defines a system that knows about <see cref="DatabaseConnection"/>.
    /// </summary>
    public interface IEventStoreConnections
    {
        /// <summary>
        /// Gets an event store connection for the given <see cref="EventStoreConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="EventStoreConfiguration"/>.</param>
        /// <returns>The <see cref="DatabaseConnection"/>.</returns>
        DatabaseConnection GetFor(EventStoreConfiguration configuration);
    }
}