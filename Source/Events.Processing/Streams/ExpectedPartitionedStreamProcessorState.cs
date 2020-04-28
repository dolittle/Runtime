// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when a partitioned <see cref="IStreamProcessorState" /> was expected but not received.
    /// </summary>
    public class ExpectedPartitionedStreamProcessorState : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedPartitionedStreamProcessorState"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        public ExpectedPartitionedStreamProcessorState(StreamProcessorId streamProcessorId)
            : base($"Expected Stream Processor: '{streamProcessorId}' to be a partitioned stream processor")
        {
        }
    }
}