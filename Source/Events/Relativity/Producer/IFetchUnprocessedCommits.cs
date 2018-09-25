using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines an interface for getting the unprocessed events for an event processor
    /// </summary>
    public interface IFetchUnprocessedCommits 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commitSequenceNumber"><see cref="CommittedEventVersion">Version</see> of the last processed event</param>
        /// <returns>A stream of commits</returns>
        Commits GetUnprocessedCommits(CommitSequenceNumber commitSequenceNumber);
    }
}