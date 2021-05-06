// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when there are no event in a stream at a specific <see cref="StreamPosition" />.
    /// </summary>
    public class NoEventInStreamAtPostion : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventInStreamAtPostion" /> class.
        /// </summary>
        /// <param name="stream">The stream that the event is not in.</param>
        /// <param name="scope">The scope that the event is not in.</param>
        /// <param name="position">The position that the event is not at.</param>
        public NoEventInStreamAtPostion(StreamId stream, ScopeId scope, StreamPosition position)
        : base($"There is no event in stream {stream.Value} in scope {scope.Value} at position {position.Value}")
        {
        }
    }
}
