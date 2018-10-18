/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Dolittle.Resources;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Relativity;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Events
{
    /// <inheritdoc/>
    public class EventStoreResourceType : IAmAResourceType
    {
        readonly IEnumerable<Type> _services = new []{typeof(IEventStore), typeof(IGeodesics), typeof(IEventProcessorOffsetRepository)};
        
        /// <inheritdoc/>
        public ResourceType Name => "eventStore";
        /// <inheritdoc/>
        public IEnumerable<Type> Services => _services;
    }
}