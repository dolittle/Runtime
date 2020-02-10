// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failing partition is being added to a stream processor but it does not exist.
    /// </summary>
    public class FailingPartitionDoesNotExist : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitionDoesNotExist"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public FailingPartitionDoesNotExist(StreamProcessorId streamProcessorId, PartitionId partitionId)
            : base($"A failing partition with partition id '{partitionId}' does not exists on stream processor '{streamProcessorId}'")
        {
        }
    }
}