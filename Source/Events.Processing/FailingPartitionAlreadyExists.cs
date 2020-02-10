// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failing partition is being added to a stream processor but it already exists.
    /// </summary>
    public class FailingPartitionAlreadyExists : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitionAlreadyExists"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public FailingPartitionAlreadyExists(StreamProcessorId streamProcessorId, PartitionId partitionId)
            : base($"A failing partition with partition id '{partitionId}' already exists on stream processor '{streamProcessorId}'")
        {
        }
    }
}