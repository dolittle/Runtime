// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.ResourceTypes;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Events
{
    /// <summary>
    /// Represents a <see cref="IAmAResourceType">resource type</see> for an event store.
    /// </summary>
    public class EventStoreResourceType : IAmAResourceType
    {
        /// <inheritdoc/>
        public ResourceType Name => "eventStore";

        /// <inheritdoc/>
        public IEnumerable<Type> Services { get; } = new[] { typeof(IEventStore), typeof(IEventProcessorOffsetRepository) };
    }
}