/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Applications;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents a null implementation of <see cref="IEventStore"/>
    /// </summary>
    public class NullEventStore : IEventStore
    {
        /// <inheritdoc/>
        public IEnumerable<EventAndEnvelope> GetFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId)
        {
            return new EventAndEnvelope[0];
        }

        /// <inheritdoc/>
        public void Commit(IEnumerable<EventAndEnvelope> eventsAndEnvelopes)
        {
        }

        /// <inheritdoc/>
        public bool HasEventsFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId)
        {
            return false;
        }

        /// <inheritdoc/>
        public EventSourceVersion GetVersionFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId)
        {
            return EventSourceVersion.Zero;
        }
    }
}
