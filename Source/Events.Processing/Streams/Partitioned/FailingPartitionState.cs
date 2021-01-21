// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
     /// <summary>
    /// Represents the state of a failing partition.
    /// </summary>
    public class FailingPartitionState : Value<FailingPartitionState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitionState"/> class.
        /// </summary>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        /// <param name="retryTime">The <see cref="DateTimeOffset" /> to retry processing.</param>
        /// <param name="reason">The reason for failing.</param>
        /// <param name="processingAttempt">The number of times the Event has been processed.</param>
        /// <param name="lastFailed">The <see cref="DateTimeOffset" /> for when this partition last failed.</param>
        public FailingPartitionState(StreamPosition position, DateTimeOffset retryTime, string reason, uint processingAttempt, DateTimeOffset lastFailed)
        {
            Position = position;
            RetryTime = retryTime;
            Reason = reason;
            ProcessingAttempts = processingAttempt;
            LastFailed = lastFailed;
        }

        /// <summary>
        /// Gets the <see cref="StreamPosition" /> of the next event to process.
        /// </summary>
        public StreamPosition Position { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> for when to retry processing.
        /// </summary>
        public DateTimeOffset RetryTime { get;  }

        /// <summary>
        /// Gets sets the reason for failure.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the number of times that the event at position has been attempted processed.
        /// </summary>
        public uint ProcessingAttempts { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> for when this partition last failed.
        /// </summary>
        public DateTimeOffset LastFailed { get; }
    }
}
