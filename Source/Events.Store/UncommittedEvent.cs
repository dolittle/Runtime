// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represent an Event that has not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvent"/> class.
        /// </summary>
        /// <param name="eventSourceId">The Events EventSourceId.</param>
        /// <param name="type">The <see cref="Artifact"/> representing the type of the Event.</param>
        /// <param name="isPublic">Whether the Event is public.</param>
        /// <param name="content">The content of the Event represented as a JSON-encoded <see cref="string"/>.</param>
        public UncommittedEvent(EventSourceId eventSourceId, Artifact type, bool isPublic, string content)
            : base(eventSourceId, type, isPublic, content)
        {
        }
    }
}
