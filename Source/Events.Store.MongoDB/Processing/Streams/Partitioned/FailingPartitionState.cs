// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents a failing partition.
    /// </summary>
    public class FailingPartitionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitionState"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="retryTime">The retry time.</param>
        /// <param name="reason">The reason for failure.</param>
        /// <param name="processingAttempts">The number of times the event at position has been processed.</param>
        /// <param name="lastFailed">The timestamp of when this partition last failed.</param>
        public FailingPartitionState(ulong position, DateTime retryTime, string reason, uint processingAttempts, DateTime lastFailed)
        {
            Position = position;
            RetryTime = retryTime;
            Reason = reason;
            ProcessingAttempts = processingAttempts;
            LastFailed = lastFailed;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong Position { get; set; }

        /// <summary>
        /// Gets or sets the retry time with Kind of UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime RetryTime { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the number of times that the event at position has been attempted processed.
        /// </summary>
        public uint ProcessingAttempts { get; set; }

        /// <summary>
        /// Gets or sets the timestamp on when the partition failed with Kind of UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastFailed { get; set; }
    }
}
