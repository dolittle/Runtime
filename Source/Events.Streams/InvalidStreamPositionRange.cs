// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Exception that gets thrown when an invalid <see cref="StreamPosition" /> range is given.
    /// </summary>
    public class InvalidStreamPositionRange : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStreamPositionRange"/> class.
        /// </summary>
        /// <param name="from">The from <see cref="StreamPosition" />.</param>
        /// <param name="to">The to <see cref="StreamPosition" />.</param>
        /// <param name="reason"> The reason why this is an illegal range.</param>
        public InvalidStreamPositionRange(StreamPosition from, StreamPosition to, string reason)
            : base($"From position '{from} and to position '{to}' is an invalid stream position range. {reason}")
        {
        }
    }
    /// <summary>
    /// Exception that gets thrown when an invalid the from <see cref="StreamPosition" /> is greater than the to <see cref="StreamPosition" /> in a <see cref="StreamPosition" /> range.
    /// </summary>
    public class FromPositionIsGreaterThanToPosition : InvalidStreamPositionRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromPositionIsGreaterThanToPosition"/> class.
        /// </summary>
        /// <param name="from">The from <see cref="StreamPosition" />.</param>
        /// <param name="to">The to <see cref="StreamPosition" />.</param>
        public FromPositionIsGreaterThanToPosition(StreamPosition from, StreamPosition to)
            : base(from, to, $"'From' position '{from} is greater than 'to' position '{to}'.")
        {
        }
    }
    /// <summary>
    /// Represents the reason for why a <see cref="StreamPositionRange" /> is invalid.
    /// </summary>
    public class InvalidStreamPositionRangeReason : ConceptAs<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStreamPositionRangeReason"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public InvalidStreamPositionRangeReason(string reason) => Value = reason;
    }

    public class StreamPositionRange : Value<StreamPositionRange>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public StreamPosition From { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public StreamPosition To { get; }
    }

}