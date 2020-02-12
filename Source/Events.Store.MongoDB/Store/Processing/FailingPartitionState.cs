// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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
        public FailingPartitionState(uint position, DateTimeOffset retryTime)
        {
            Position = position;
            RetryTime = retryTime;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public uint Position { get; set; }

        /// <summary>
        /// Gets or sets the retry time.
        /// </summary>
        public DateTimeOffset RetryTime { get; set; }
    }
}