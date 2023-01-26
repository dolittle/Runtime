// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents the state of an <see cref="StreamProcessor" />.
/// </summary>
/// <param name="StreamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
/// <param name="FailingPartitions">The <see cref="IDictionary{PartitionId, FailingPartitionState}">states of the failing partitions</see>.</param>
/// <param name="LastSuccessfullyProcessed">The <see cref="DateTimeOffset" /> for the last time when an Event in the Stream that the <see cref="ScopedStreamProcessor" /> processes was processed successfully.</param>
public record StreamProcessorState(StreamPosition Position, IDictionary<PartitionId, FailingPartitionState> FailingPartitions, DateTimeOffset LastSuccessfullyProcessed) : IStreamProcessorState
{
    /// <summary>
    /// Gets a new, initial, <see cref="StreamProcessorState" />.
    /// </summary>
    public static StreamProcessorState New => new(StreamPosition.Start, new Dictionary<PartitionId, FailingPartitionState>(), DateTimeOffset.MinValue);

    /// <inheritdoc/>
    public bool Partitioned => true;
}