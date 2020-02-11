// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the identification of an event source.
    /// </summary>
    public class EventSourceId : ConceptAs<Guid>
    {
        /// <summary>
        /// A static singleton instance to represent a "NotSet" <see cref="EventSourceId" />.
        /// </summary>
        public static readonly EventSourceId NotSet = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceId"/> class.
        /// </summary>
        public EventSourceId()
            : this(Guid.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceId"/> class.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> value.</param>
        public EventSourceId(Guid id) => Value = id;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to an <see cref="EventSourceId"/>.
        /// </summary>
        /// <param name="eventId">EventSourceId as <see cref="Guid"/>.</param>
        public static implicit operator EventSourceId(Guid eventId) => new EventSourceId { Value = eventId };

        /// <summary>
        /// Creates a new instance of <see cref="EventSourceId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="EventSourceId"/>.</returns>
        public static EventSourceId New()
        {
            return new EventSourceId { Value = Guid.NewGuid() };
        }
    }
}
