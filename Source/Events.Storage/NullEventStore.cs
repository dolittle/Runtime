/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents a null implementation of <see cref="IEventStore"/>
    /// </summary>
    public class NullEventStore : IEventStore
    {
        /// <inheritdoc/>
        public IEnumerable<EventAndEnvelope> GetFor(Artifact eventSource, EventSourceId eventSourceId)
        {
            return new EventAndEnvelope[0];
        }

        /// <inheritdoc/>
        public void Commit(IEnumerable<EventAndEnvelope> eventsAndEnvelopes)
        {
        }

        /// <inheritdoc/>
        public bool HasEventsFor(Artifact eventSource, EventSourceId eventSourceId)
        {
            return false;
        }

        /// <inheritdoc/>
        public EventSourceVersion GetVersionFor(Artifact eventSource, EventSourceId eventSourceId)
        {
            return EventSourceVersion.Zero;
        }
    }
}
