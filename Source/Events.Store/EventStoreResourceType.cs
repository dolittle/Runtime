// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a <see cref="IAmAResourceType">resource type</see> for an event store.
    /// </summary>
    public class EventStoreResourceType : IAmAResourceType
    {
        /// <inheritdoc/>
        public ResourceType Name => "eventStore";

        /// <inheritdoc/>
        public IEnumerable<Type> Services { get; } = new[]
        {
            typeof(IEventStore),
            typeof(IFetchAggregateRootInstances),
            typeof(IStreamDefinitionRepository),
            typeof(IStreamProcessorStateRepository),
            typeof(IEventFetchers),
            typeof(IWriteEventsToStreams),
            typeof(IWriteEventHorizonEvents),
            typeof(IWriteEventsToPublicStreams),
        };
    }
}
