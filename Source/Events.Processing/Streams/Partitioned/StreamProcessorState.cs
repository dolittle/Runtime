// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
    /// <summary>
    /// Represents the state of an <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorState : Value<StreamProcessorState>, IStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
        /// <param name="failingPartitions">The <see cref="IDictionary{PartitionId, FailingPartitionState}">states of the failing partitions</see>.</param>
        /// <param name="lastSuccessfullyProcessed">The <see cref="DateTimeOffset" /> for the last time when an Event in the Stream that the <see cref="ScopedStreamProcessor" /> processes was processed successfully.</param>
        public StreamProcessorState(StreamPosition streamPosition, IDictionary<PartitionId, FailingPartitionState> failingPartitions, DateTimeOffset lastSuccessfullyProcessed)
        {
            Position = streamPosition;
            FailingPartitions = failingPartitions;
            LastSuccessfullyProcessed = lastSuccessfullyProcessed;
        }

        /// <summary>
        /// Gets a new, initial, <see cref="StreamProcessorState" />.
        /// </summary>
        public static StreamProcessorState New =>
            new StreamProcessorState(StreamPosition.Start, new Dictionary<PartitionId, FailingPartitionState>(), DateTimeOffset.MinValue);

        /// <inheritdoc/>
        public bool Partitioned => true;

        /// <summary>
        /// Gets the <see cref="StreamPosition" />.
        /// </summary>
        public StreamPosition Position { get; }

        /// <summary>
        /// Gets the <see cref="IDictionary{PartitionId, FailingPartitionState}">states of the failing partitions</see>.
        /// </summary>
        public IDictionary<PartitionId, FailingPartitionState> FailingPartitions { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> for the last time when an Event in the Stream that the <see cref="ScopedStreamProcessor" /> processes was processed successfully.
        /// </summary>
        public DateTimeOffset LastSuccessfullyProcessed { get; }
    }
}
