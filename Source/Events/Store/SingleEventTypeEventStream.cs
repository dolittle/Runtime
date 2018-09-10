using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Events;
using Dolittle.Artifacts;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// An enumerable of <see cref="CommittedEventEnvelope" />s for holding committed events that cross commits
    /// </summary>
    public class SingleEventTypeEventStream : IEnumerable<CommittedEventEnvelope>, IEquatable<SingleEventTypeEventStream>
    {
        List<CommittedEventEnvelope> _events;
        private static readonly EnumerableEqualityComparer<CommittedEventEnvelope> _comparer = new EnumerableEqualityComparer<CommittedEventEnvelope>();

        /// <summary>
        /// Instantiates a new instance of <see cref="SingleEventTypeEventStream" /> initialized with events.
        /// Event instances of more than one <see cref="ArtifactId">type</see> throws a <see cref="MultipleEventTypesInSingleEventTypeEventStream" /> exception
        /// </summary>
        /// <param name="events"></param>
        public SingleEventTypeEventStream(IEnumerable<CommittedEventEnvelope> events)
        {
            if(events != null && events.Any())
            {
                var artifact = events.First().Metadata.Artifact.Id;
                if(!events.All(e => e.Metadata.Artifact.Id == artifact))
                    throw new MultipleEventTypesInSingleEventTypeEventStream();
            }

            _events = events != null ? new List<CommittedEventEnvelope>(events) : new List<CommittedEventEnvelope>();
        }

        /// <summary>
        /// Equates two <see cref="SingleEventTypeEventStream" /> instances
        /// </summary>
        /// <param name="first">The first <see cref="SingleEventTypeEventStream" /> instance</param>
        /// <param name="second">The second <see cref="SingleEventTypeEventStream" /> instance</param>
        /// <returns>true if equal, otherwise false</returns>
        public static bool Equals(SingleEventTypeEventStream first, SingleEventTypeEventStream second)
        {
            return _comparer.Equals(first,second);
        }

        /// <summary>
        /// Equates this instance of <see cref="SingleEventTypeEventStream" /> to another
        /// </summary>
        /// <param name="other"> The other <see cref="SingleEventTypeEventStream" /> to equate with</param>
        /// <returns>true if equal, false otherwise</returns>
        public bool Equals(SingleEventTypeEventStream other)
        {
            return Equals(this,other);
        }

        /// <summary>
        /// Gets an enumerator for iterating over the <see cref="CommittedEventEnvelope" />s
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CommittedEventEnvelope> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        /// <summary>
        /// Calculates the hashcode for this 
        /// </summary>
        /// <returns>the hashcode</returns>
        public override int GetHashCode()
        {
            return _comparer.GetHashCode(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator() as IEnumerator;
        }

        /// <summary>
        /// Indicates if there are any events
        /// </summary>
        public bool IsEmpty => !_events.Any();
    }
}


