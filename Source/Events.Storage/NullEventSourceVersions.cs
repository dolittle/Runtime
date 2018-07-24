/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventSourceVersions"/>
    /// </summary>
    public class NullEventSourceVersions : IEventSourceVersions
    {
        readonly IEventStore _eventStore;

        /// <summary>
        /// Initializes a new instance of <see cref="NullEventSourceVersions"/>
        /// </summary>
        /// <param name="eventStore"><see cref="IEventStore"/> to forward requests for versioning</param>
        public NullEventSourceVersions(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }


        /// <inheritdoc/>
        public EventSourceVersion GetFor(Artifact eventSource, EventSourceId eventSourceId)
        {
            var version = EventSourceVersion.Zero;
            version = _eventStore.GetVersionFor(eventSource, eventSourceId);
            return version;
        }

        /// <inheritdoc/>
        public void SetFor(Artifact eventSource, EventSourceId eventSourceId, EventSourceVersion version)
        {
        }
    }
}