// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <inheritdoc/>
    public class EventStoreResourceTypeRepresentation : IRepresentAResourceType
    {
        static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            { typeof(IEventStore), typeof(EventStore) },
            { typeof(IFetchAggregateRootInstances), typeof(AggregateRootInstancesFetcher) },
            { typeof(IStreamDefinitionRepository), typeof(StreamDefinitionRepository) },
            { typeof(IStreamProcessorStateRepository), typeof(StreamProcessorStateRepository) },
            { typeof(IEventFetchers), typeof(EventFetchers) },
            { typeof(IWriteEventsToStreams), typeof(EventsToStreamsWriter) },
            { typeof(IWriteEventHorizonEvents), typeof(EventHorizonEventsWriter) },
            { typeof(IWriteEventsToPublicStreams), typeof(EventsToPublicStreamsWriter) }
        };

        /// <inheritdoc/>
        public ResourceType Type => "eventStore";

        /// <inheritdoc/>
        public ResourceTypeImplementation ImplementationName => "MongoDB";

        /// <inheritdoc/>
        public Type ConfigurationObjectType => typeof(EventStoreConfiguration);

        /// <inheritdoc/>
        public IDictionary<Type, Type> Bindings => _bindings;
    }
}
