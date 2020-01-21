// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchUnprocessedEvents" />.
    /// </summary>
    public class FetchUnprocessedEvents : IFetchUnprocessedEvents
    {
        readonly FactoryFor<IEventStore> _getEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchUnprocessedEvents"/> class.
        /// </summary>
        /// <param name="getEventStore"><see cref="FactoryFor{T}"/> for <see cref="IEventStore"/>.</param>
        public FetchUnprocessedEvents(FactoryFor<IEventStore> getEventStore)
        {
            _getEventStore = getEventStore;
        }

        /// <inheritdoc />
        public SingleEventTypeEventStream GetUnprocessedEvents(ArtifactId eventType, CommittedEventVersion committedEventVersion)
        {
            using (var eventStore = _getEventStore())
            {
                var eventStream = eventStore.FetchAllEventsOfTypeAfter(eventType, committedEventVersion.Major);
                return new SingleEventTypeEventStream(eventStream.Where(e => e.Version > committedEventVersion));
            }
        }
    }
}