// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Events.Store.Streams;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents the state of an <see cref="StreamProcessor" />.
/// </summary>
/// <param name="StreamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
/// <param name="FailingPartitions">The <see cref="IDictionary{PartitionId, FailingPartitionState}">states of the failing partitions</see>.</param>
/// <param name="LastSuccessfullyProcessed">The <see cref="DateTimeOffset" /> for the last time when an Event in the Stream that the <see cref="ScopedStreamProcessor" /> processes was processed successfully.</param>
public record StreamProcessorState(StreamPosition Position, IDictionary<PartitionId, FailingPartitionState> FailingPartitions,
    DateTimeOffset LastSuccessfullyProcessed) : IStreamProcessorState
{
    /// <summary>
    /// Gets a new, initial, <see cref="StreamProcessorState" />.
    /// </summary>
    public static StreamProcessorState New => new(StreamPosition.Start, new Dictionary<PartitionId, FailingPartitionState>(), DateTimeOffset.MinValue);

    /// <inheritdoc/>
    public bool Partitioned => true;

    public Bucket ToProtobuf() => new()
    {
        BucketId = 0,
        CurrentOffset = Position.Value,
        LastSuccessfullyProcessed = Timestamp.FromDateTimeOffset(LastSuccessfullyProcessed),
        Failures =
        {
            FailingPartitions.Select(_ =>
            {
                var (eventSourceId, failingPartitionState) = _;
                return new ProcessingFailure
                {
                    EventSourceId = eventSourceId,
                    Offset = _.Value.Position.Value,
                    FailureReason = failingPartitionState.Reason,
                    ProcessingAttempts = failingPartitionState.ProcessingAttempts,
                    RetryTime = Timestamp.FromDateTimeOffset(failingPartitionState.RetryTime),
                    LastFailed = Timestamp.FromDateTimeOffset(failingPartitionState.LastFailed)
                };
            })
        },
        Partitioned = true
    };
}
