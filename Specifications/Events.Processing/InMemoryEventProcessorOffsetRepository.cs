// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Specs.Processing
{
    public class InMemoryEventProcessorOffsetRepository : IEventProcessorOffsetRepository
    {
        ConcurrentDictionary<EventProcessorId, CommittedEventVersion> _offsets = new ConcurrentDictionary<EventProcessorId, CommittedEventVersion>();

        public void Dispose()
        {
        }

        public CommittedEventVersion Get(EventProcessorId eventProcessorId)
        {
            return _offsets.GetValueOrDefault(eventProcessorId) ?? CommittedEventVersion.None;
        }

        public void Set(EventProcessorId eventProcessorId, CommittedEventVersion committedEventVersion)
        {
            _offsets.AddOrUpdate(eventProcessorId, committedEventVersion, (id, vsn) => committedEventVersion);
        }
    }
}