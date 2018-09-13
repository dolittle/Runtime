using System;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{

    /// <summary>
    /// Implemenation of <see cref="IFetchUnprocessedEvents" />
    /// </summary>
    public class FetchUnprocessedEvents : IFetchUnprocessedEvents
    {
        FactoryFor<IEventStore> _getEventStore;

        /// <summary>
        /// Instantiates an instance of <see cref="FetchUnprocessedEvents" />
        /// </summary>
        /// <param name="getEventStore"></param>
        public FetchUnprocessedEvents(FactoryFor<IEventStore> getEventStore)
        {
            _getEventStore = getEventStore;
        }

        /// <inheritdoc />
        public SingleEventTypeEventStream GetUnprocessedEvents(ArtifactId eventType, CommittedEventVersion committedEventVersion)
        {
            using(var eventStore = _getEventStore())
            {
                var eventStream = eventStore.FetchAllEventsOfTypeAfter(eventType,committedEventVersion.Major);
                return new SingleEventTypeEventStream(eventStream.Where(e => e.Version > committedEventVersion));
            }
        }
    }
}