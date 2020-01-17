// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents a system that can process a stream of <see cref="EventEnvelope">events</see>.
    /// </summary>
    public interface ICanProcessStreamOfEvents
    {
        /// <summary>
        /// Processes an <see cref="EventEnvelope">event</see>.
        /// </summary>
        /// <param name="eventEnvelope">The <see cref="EventEnvelope">event</see>.</param>
        /// <returns>The state of the last processed <see cref="EventEnvelope">event</see>.</returns>
        EventProcessorState Process(EventEnvelope eventEnvelope);
    }
}