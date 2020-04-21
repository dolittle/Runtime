// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the basis for an event.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="eventSourceId">The Events EventSourceId.</param>
        /// <param name="type">The <see cref="Artifact"/> representing the type of the Event.</param>
        /// <param name="isPublic">Whether the Event is public.</param>
        /// <param name="content">The content of the Event represented as a JSON-encoded <see cref="string"/>.</param>
        protected Event(EventSourceId eventSourceId, Artifact type, bool isPublic, string content)
        {
            EventSource = eventSourceId;
            Type = type;
            Public = isPublic;
            Content = content;
        }

        /// <summary>
        /// Gets the <see cref="EventSource" />. that the Event was applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets the <see cref="Artifact"/> representing the type of the Event.
        /// </summary>
        public Artifact Type { get; }

        /// <summary>
        /// Gets a value indicating whether the Event is public.
        /// </summary>
        public bool Public { get; }

        /// <summary>
        /// Gets the content of the Event represented as a JSON-encoded <see cref="string"/>.
        /// </summary>
        public string Content { get; }
    }
}
