// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Concepts;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Unpartitioned
{
    /// <summary>
    /// Represents a combination of <see cref="StreamPosition" /> and <see cref="IDictionary{PartitionId, FailingPartitionState}">states of failing partitions</see> that represents the state of an <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorState : Value<StreamProcessorState>, IStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
        public StreamProcessorState(StreamPosition streamPosition)
        {
            Position = streamPosition;
            IsFailing = false;
            RetryTime = DateTimeOffset.UtcNow;
            FailureReason = string.Empty;
            ProcessingAttempts = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
        /// <param name="failureReason">The reason for failing.</param>
        /// <param name="retryTime">The <see cref="DateTimeOffset" /> for when to retry processing.</param>
        /// <param name="processingAttempts">The number of times it has processed the Event at <see cref="Position" />.</param>
        public StreamProcessorState(StreamPosition streamPosition, string failureReason, DateTimeOffset retryTime, uint processingAttempts)
        {
            Position = streamPosition;
            IsFailing = true;
            RetryTime = retryTime;
            FailureReason = failureReason;
            ProcessingAttempts = processingAttempts;
        }

        /// <summary>
        /// Gets a new, initial, <see cref="StreamProcessorState" />.
        /// </summary>
        public static StreamProcessorState New =>
            new StreamProcessorState(StreamPosition.Start);

        /// <inheritdoc/>
        public bool Partitioned => false;

        /// <inheritdoc/>
        public StreamPosition Position { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="StreamProcessor" /> is failing.
        /// </summary>
        public bool IsFailing { get; }

        /// <summary>
        /// Gets or sets the <see cref="DateTimeOffset" /> for when to retry processing.
        /// </summary>
        public DateTimeOffset RetryTime { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure.
        /// </summary>
        public string FailureReason { get; set; }

        /// <summary>
        /// Gets or sets the number of times that the event at position has been attempted processed.
        /// </summary>
        public uint ProcessingAttempts { get; set; }
    }
}
