// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="StreamEvent"/> has a different <see cref="PartitionId"/> than expected.
    /// </summary>
    public class StreamEventInWrongPartition : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamEventInWrongPartition"/> class.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent"/> with the wrong <see cref="PartitionId"/>.</param>
        /// <param name="expectedPartitionId">The expected <see cref="PartitionId"/>.</param>
        public StreamEventInWrongPartition(StreamEvent @event, PartitionId expectedPartitionId)
            : base($"Expected Event at Position: {@event.Position} in Stream: '{@event.Stream}' to be in Partition: '{expectedPartitionId}' but was in Partition: {@event.Partition}")
        {
        }
    }
}
