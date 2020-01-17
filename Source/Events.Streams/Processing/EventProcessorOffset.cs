// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// An incrementing number used to identify the offset of an event processor.
    /// </summary>
    public class EventProcessorOffset : ConceptAs<ulong>
    {
        /// <summary>
        /// Represents a start of the <see cref="EventProcessorOffset">offset</see>.
        /// </summary>
        public static EventProcessorOffset Start = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorOffset"/> class.
        /// </summary>
        /// <param name="offset">Value representing.</param>
        public EventProcessorOffset(ulong offset) => Value = offset;

        /// <summary>
        /// Implicitly convert from <see cref="ulong" /> to <see cref="EventProcessorOffset" />.
        /// </summary>
        /// <param name="offset">Offset number as <see cref="ulong"/>.</param>
        public static implicit operator EventProcessorOffset(ulong offset) => new EventProcessorOffset(offset);
    }
}