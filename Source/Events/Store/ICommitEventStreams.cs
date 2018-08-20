using System;
using System.Collections.Generic;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Defines an interface for committing event streams
    /// </summary>
    public interface ICommitEventStreams 
    {
        /// <summary>
        /// Commits an <see cref="UncommittedEventStream" /> to the Event Store, returning a corresponding <see cref="CommittedEventStream" />
        /// with a unique <see cref="CommitSequenceNumber" />
        /// </summary>
        /// <param name="uncommittedEvents">The <see cref="UncommittedEventStream" /> to be committed</param>
        /// <returns>A <see cref="CommittedEventStream" /> corresponding to the <see cref="UncommittedEventStream" /> supplied</returns>
        CommittedEventStream Commit(UncommittedEventStream uncommittedEvents);
    }
}