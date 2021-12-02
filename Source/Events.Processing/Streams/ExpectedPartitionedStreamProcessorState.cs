// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Exception that gets thrown when a partitioned <see cref="IStreamProcessorState" /> was expected but not received.
/// </summary>
public class ExpectedPartitionedStreamProcessorState : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectedPartitionedStreamProcessorState"/> class.
    /// </summary>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    public ExpectedPartitionedStreamProcessorState(IStreamProcessorId streamProcessorId)
        : base($"Expected Stream Processor: '{streamProcessorId}' to be a partitioned stream processor")
    {
    }
}