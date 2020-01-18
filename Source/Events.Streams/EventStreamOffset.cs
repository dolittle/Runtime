// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// An offset used to identify the offset of an event stream.
    /// </summary>
    public class EventStreamOffset : ConceptAs<ulong>
    {
        /// <summary>
        /// Represents a start of the <see cref="EventStreamOffset">offset</see>.
        /// </summary>
        public static EventStreamOffset Start = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamOffset"/> class.
        /// </summary>
        /// <param name="offset">Value representing.</param>
        public EventStreamOffset(ulong offset) => Value = offset;

        /// <summary>
        /// Implicitly convert from <see cref="ulong" /> to <see cref="EventStreamOffset" />.
        /// </summary>
        /// <param name="offset">Offset number as <see cref="ulong"/>.</param>
        public static implicit operator EventStreamOffset(ulong offset) => new EventStreamOffset(offset);
    }
}