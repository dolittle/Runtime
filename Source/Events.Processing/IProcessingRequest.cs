// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the processing request.
    /// </summary>
    public interface IProcessingRequest
    {
        /// <summary>
        /// Gets the <see cref="CommittedEvent" />.
        /// </summary>
        CommittedEvent Event { get; }

        /// <summary>
        /// Gets the <see cref="PartitionId" />.
        /// </summary>
        PartitionId Partition { get; }

        /// <summary>
        /// Gets the <see cref="ExecutionContext" />.
        /// </summary>
        ExecutionContext ExecutionContext { get; }
    }
}