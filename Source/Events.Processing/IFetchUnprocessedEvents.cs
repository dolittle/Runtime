// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines an interface for getting the unprocessed events for an event processor.
    /// </summary>
    public interface IFetchUnprocessedEvents
    {
        /// <summary>
        /// Get unprocessed events for a specific <see cref="ArtifactId">event type</see>.
        /// </summary>
        /// <param name="eventType"><see cref="ArtifactId">Event Type</see> to fetch.</param>
        /// <param name="committedEventVersion"><see cref="CommittedEventVersion">Version</see> of the last processed event.</param>
        /// <returns>A stream of unprocessed events.</returns>
        SingleEventTypeEventStream GetUnprocessedEvents(ArtifactId eventType, CommittedEventVersion committedEventVersion);
    }
}