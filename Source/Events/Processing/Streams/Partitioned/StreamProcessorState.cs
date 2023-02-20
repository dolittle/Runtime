// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Events.Store.Streams;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents the state of an <see cref="StreamProcessor" />.
/// </summary>
/// <param name="Position">The <see cref="StreamPosition"/>position of the stream.</param>
/// <param name="EventLogPosition">The <see cref="StreamPosition"/>position of the stream.</param>
/// <param name="FailingPartitions">The <see cref="IDictionary{PartitionId, FailingPartitionState}">states of the failing partitions</see>.</param>
/// <param name="LastSuccessfullyProcessed">The <see cref="DateTimeOffset" /> for the last time when an Event in the Stream that the <see cref="ScopedStreamProcessor" /> processes was processed successfully.</param>
public record StreamProcessorState(ProcessingPosition Position, ImmutableDictionary<PartitionId, FailingPartitionState> FailingPartitions,
    DateTimeOffset LastSuccessfullyProcessed) : IStreamProcessorState
{
    // public StreamProcessorState(ProcessingPosition position, IDictionary<PartitionId, FailingPartitionState> failingPartitions, DateTimeOffset lastSuccessfullyProcessed)
    //     : this(position.StreamPosition, position.EventLogPosition, failingPartitions, lastSuccessfullyProcessed){}

    /// <summary>
    /// Gets a new, initial, <see cref="StreamProcessorState" />.
    /// </summary>
    public static StreamProcessorState New =>
        new(ProcessingPosition.Initial, ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, DateTimeOffset.MinValue);

    /// <inheritdoc/>
    public bool Partitioned => true;

    // public ProcessingPosition ProcessingPosition => new(Position, EventLogPosition);

    public Bucket ToProtobuf() => new()
    {
        BucketId = 0,
        CurrentOffset = Position.StreamPosition.Value,
        CurrentEventLogOffset = Position.EventLogPosition.Value,
        LastSuccessfullyProcessed = Timestamp.FromDateTimeOffset(LastSuccessfullyProcessed),
        Failures =
        {
            FailingPartitions.Select(_ =>
            {
                var (eventSourceId, failingPartitionState) = _;
                return new ProcessingFailure
                {
                    EventSourceId = eventSourceId,
                    Offset = _.Value.Position.StreamPosition.Value,
                    EventLogOffset = _.Value.Position.EventLogPosition.Value,
                    FailureReason = failingPartitionState.Reason,
                    ProcessingAttempts = failingPartitionState.ProcessingAttempts,
                    RetryTime = Timestamp.FromDateTimeOffset(failingPartitionState.RetryTime),
                    LastFailed = Timestamp.FromDateTimeOffset(failingPartitionState.LastFailed)
                };
            })
        },
        Partitioned = true
    };

    /// <summary>
    /// Returns earliest processable position.
    /// Will Skip over failing partitions without a retry time.
    /// </summary>
    public ProcessingPosition EarliestProcessingPosition => FailingPartitions.Any() ? FailingPartitions.Where(_ => _.Value.CanBeRetried).Min(_ => _.Value.Position)! : Position;

    public bool TryGetTimespanToRetry(out TimeSpan timeToRetry)
    {
        timeToRetry = TimeSpan.MaxValue;
        var failingStates = FailingPartitions;
        if (failingStates.Count > 0)
        {
            var earliestRetryTime = failingStates.Min(_ => _.Value.RetryTime);
            timeToRetry = RetryTimeIsInThePast(earliestRetryTime) ? TimeSpan.Zero : earliestRetryTime.Subtract(DateTimeOffset.UtcNow);
            return true;
        }

        return false;
    }

    public IStreamProcessorState WithFailure(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retriedAt = default)
    {
        if (failedProcessing.Succeeded)
        {
            throw new ArgumentException("Processing result cannot be successful when adding a failing partition", nameof(failedProcessing));
        }
        
        if(FailingPartitions.TryGetValue(processedEvent.Partition, out var failingPartitionState))
        {
            return UpdateFailingPartition(failedProcessing, processedEvent, retriedAt, failingPartitionState);
        }

        return AddFailingPartitionFor(
            this,
            processedEvent.CurrentProcessingPosition,
            processedEvent.Partition,
            retriedAt == default ? DateTimeOffset.MaxValue : retriedAt,
            failedProcessing.FailureReason);
    }

    IStreamProcessorState UpdateFailingPartition(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retriedAt,
        FailingPartitionState failingPartitionState)
    {
        var failingPartition = new FailingPartitionState(
            Position: processedEvent.CurrentProcessingPosition,
            RetryTime: failedProcessing.Retry ? DateTimeOffset.UtcNow.Add(failedProcessing.RetryTimeout) : DateTimeOffset.MaxValue,
            ProcessingAttempts: processedEvent.CurrentProcessingPosition.EventLogPosition == failingPartitionState.Position.EventLogPosition
                ? failingPartitionState.ProcessingAttempts + 1
                : 1,
            Reason: failedProcessing.FailureReason,
            LastFailed: retriedAt);
        return this with
        {
            FailingPartitions = FailingPartitions.SetItem(processedEvent.Partition, failingPartition)
        };
    }

    public IStreamProcessorState WithSuccessfullyProcessed(StreamEvent processedEvent, DateTimeOffset timestamp)
    {
        if (FailingPartitions.TryGetValue(processedEvent.Partition, out var failingPartitionState))
        {
            if (Position.EventLogPosition <= processedEvent.CurrentProcessingPosition.EventLogPosition)
            {
                // Since the event log position is the same or higher than the failing partition state, we can remove the failing partition state
                return new StreamProcessorState(Position: processedEvent.NextProcessingPosition,
                    FailingPartitions: FailingPartitions.Remove(processedEvent.Partition), LastSuccessfullyProcessed: timestamp);
            }

            // There might be more events between this event and the current high watermark of processed events.
            var failingPartition = failingPartitionState with
            {
                Position = processedEvent.NextProcessingPosition,
                RetryTime = DateTimeOffset.UtcNow,
                ProcessingAttempts = 0,
                Reason = "behind"
            };
            return new StreamProcessorState(Position: processedEvent.NextProcessingPosition,
                FailingPartitions: FailingPartitions.SetItem(processedEvent.Partition, failingPartition), LastSuccessfullyProcessed: timestamp);
        }

        return this with
        {
            Position = processedEvent.NextProcessingPosition,
            LastSuccessfullyProcessed = timestamp
        };
    }

    static IStreamProcessorState AddFailingPartitionFor(
        StreamProcessorState oldState,
        ProcessingPosition failedPosition,
        PartitionId partition,
        DateTimeOffset retryTime,
        string reason)
    {
        var failingPartition = new FailingPartitionState(failedPosition, retryTime, reason, 1, DateTimeOffset.UtcNow);
        var newState = new StreamProcessorState(failedPosition.IncrementWithStream(), oldState.FailingPartitions.SetItem(partition, failingPartition),
            oldState.LastSuccessfullyProcessed);
        return newState;
    }

    bool RetryTimeIsInThePast(DateTimeOffset retryTime)
        => DateTimeOffset.UtcNow.CompareTo(retryTime) >= 0;

    /// <summary>
    /// Remove the previously failing partitions that are no longer failing.
    /// </summary>
    /// <param name="noLongerFailingPartitions"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public StreamProcessorState WithoutFailingPartitions(params PartitionId[] noLongerFailingPartitions) =>
        this with
        {
            FailingPartitions = FailingPartitions.RemoveRange(noLongerFailingPartitions)
        };
    
    public StreamProcessorState WithFailingPartition(PartitionId partitionId, FailingPartitionState partitionState) =>
        this with
        {
            FailingPartitions = FailingPartitions.SetItem(partitionId, partitionState)
        };
}
