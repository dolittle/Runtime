// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.ResourceTypes.Configuration;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IConfigurationFor{T}"/> for <see cref="EventStoreConfiguration"/>.
    /// </summary>
    public class EventStoreDatabaseConfiguration : IConfigurationFor<EventStoreConfiguration>
    {
        public EventStoreConfiguration Instance { get; init; }
    }
}