// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represent an Event that has not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvent"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Artifact"/> representing the type of the Event.</param>
        /// <param name="content">The content of the Event represented as a JSON-encoded <see cref="string"/>.</param>
        public UncommittedEvent(Artifact type, string content)
        {
            Type = type;
            Content = content;
        }

        /// <summary>
        /// Gets the <see cref="Artifact"/> representing the type of the Event.
        /// </summary>
        public Artifact Type { get; }

        /// <summary>
        /// Gets the content of the Event represented as a JSON-encoded <see cref="string"/>.
        /// </summary>
        public string Content { get; }
    }
}