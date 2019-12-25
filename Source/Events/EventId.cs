// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the identification of an <see cref="IEvent"/>.
    /// </summary>
    public class EventId : ConceptAs<Guid>
    {
        /// <summary>
        /// Represents a not set <see cref="EventId"/>.
        /// </summary>
        public static EventId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="long"/> to an <see cref="EventId"/>.
        /// </summary>
        /// <param name="eventId"><see cref="Guid"/> representation.</param>
        public static implicit operator EventId(Guid eventId) => new EventId { Value = eventId };

        /// <summary>
        /// Creates a new instance of <see cref="EventId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="EventId"/>.</returns>
        public static EventId New()
        {
            return new EventId { Value = Guid.NewGuid() };
        }
    }
}
