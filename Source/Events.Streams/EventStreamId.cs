// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents the identification of an event stream.
    /// </summary>
    public class EventStreamId : ConceptAs<Guid>
    {
        /// <summary>
        /// Represents a not set <see cref="EventStreamId"/>.
        /// </summary>
        public static EventStreamId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="long"/> to an <see cref="EventStreamId"/>.
        /// </summary>
        /// <param name="eventStreamId"><see cref="Guid"/> representation.</param>
        public static implicit operator EventStreamId(Guid eventStreamId) => new EventStreamId { Value = eventStreamId };

        /// <summary>
        /// Creates a new instance of <see cref="EventStreamId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="EventStreamId"/>.</returns>
        public static EventStreamId New()
        {
            return new EventStreamId { Value = Guid.NewGuid() };
        }
    }
}
