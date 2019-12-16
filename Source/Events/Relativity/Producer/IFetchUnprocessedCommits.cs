// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines an interface for getting the unprocessed events for an event processor.
    /// </summary>
    public interface IFetchUnprocessedCommits
    {
        /// <summary>
        /// Get unprocessed commits.
        /// </summary>
        /// <param name="commitSequenceNumber"><see cref="CommittedEventVersion">Version</see> of the last processed event.</param>
        /// <returns>A stream of commits.</returns>
        Commits GetUnprocessedCommits(CommitSequenceNumber commitSequenceNumber);
    }
}