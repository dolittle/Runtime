// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.ResourceTypes;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <inheritdoc/>
    public class EventStoreResourceTypeRepresentation : IRepresentAResourceType
    {
        static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            { typeof(Dolittle.Runtime.Events.Store.IEventStore), typeof(Store.MongoDB.EventStore) },
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