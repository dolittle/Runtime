// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
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
        public FailingPartitionState(uint position, DateTimeOffset retryTime, string reason)
        {
            Position = position;
            RetryTime = retryTime;
            Reason = reason;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public uint Position { get; set; }

        /// <summary>
        /// Gets or sets the retry time.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset RetryTime { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure.
        /// </summary>
        public string Reason { get; set; }
    }
}