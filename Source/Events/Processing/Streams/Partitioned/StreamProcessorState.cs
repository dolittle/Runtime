// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dolittle.Runtime.Events.Store;
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
    DateTimeOffset LastSuccessfullyProcessed) : IStreamProcessorState<StreamProcessorState>
{
    public StreamProcessorState(ProcessingPosition position, DateTimeOffset lastSuccessfullyProcessed) : this(position,
        ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, lastSuccessfullyProcessed)
    {
    }

    [Obsolete("legacy stream processor state, without event log position")]
    public StreamProcessorState(StreamPosition streamPosition, ImmutableDictionary<PartitionId, FailingPartitionState> failingPartitions,
        DateTimeOffset lastSuccessfullyProcessed) :
        this(new ProcessingPosition(streamPosition, EventLogSequenceNumber.Initial), failingPartitions, lastSuccessfullyProcessed)
    {
    }

    /// <summary>
    /// Gets a new, initial, <see cref="StreamProcessorState" />.
    /// </summary>
    public static StreamProcessorState New =>
        new(ProcessingPosition.Initial, ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, DateTimeOffset.MinValue);

    /// <inheritdoc/>
    public bool Partitioned => true;

    public int FailingPartitionCount => FailingPartitions.Count;
    
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
    public ProcessingPosition EarliestProcessingPosition =>
        FailingPartitions.Any() ? FailingPartitions.Where(_ => _.Value.CanBeRetried).Min(_ => _.Value.Position)! : Position;

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


    public StreamProcessorState WithResult(IProcessingResult result, StreamEvent processedEvent, DateTimeOffset timestamp)
    {
        VerifyEventHasValidProcessingPosition(processedEvent);

        if (result.Retry)
        {
            return WithFailure(result, processedEvent, timestamp.Add(result.RetryTimeout), timestamp);
        }

        if (result is SkippedProcessing)
        {
            return this with { Position = Position.IncrementWithStream() };
        }

        return result.Succeeded
            ? WithSuccessfullyProcessed(processedEvent, timestamp)
            : WithFailure(result, processedEvent, DateTimeOffset.MaxValue, timestamp);
    }

    public StreamProcessorState WithFailure(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retryAt,
        DateTimeOffset lastProcessingAttempt = default)
    {
        if (failedProcessing.Succeeded)
        {
            throw new ArgumentException("Processing result cannot be successful when adding a failing partition", nameof(failedProcessing));
        }

        if (FailingPartitions.TryGetValue(processedEvent.Partition, out var failingPartitionState))
        {
            return UpdateFailingPartition(failedProcessing, processedEvent, lastProcessingAttempt, failingPartitionState);
        }

        return AddFailingPartitionFor(
            this,
            processedEvent.CurrentProcessingPosition,
            processedEvent.Partition,
            retryAt == default ? DateTimeOffset.MaxValue : retryAt,
            failedProcessing.FailureReason,
            lastProcessingAttempt == default ? DateTimeOffset.UtcNow : lastProcessingAttempt);
    }

    void VerifyEventHasValidProcessingPosition(StreamEvent processedEvent)
    {
        if (processedEvent.CurrentProcessingPosition.StreamPosition == Position.StreamPosition)
        {
            return;
        }

        if (!FailingPartitions.TryGetValue(processedEvent.Partition, out var failingPartitionState))
        {
            throw new ArgumentException(
                $"The processed event does not match the current position of the stream processor. Expected {Position}, got {processedEvent.CurrentProcessingPosition}",
                nameof(processedEvent));
        }

        if (failingPartitionState.Position.StreamPosition <= processedEvent.CurrentProcessingPosition.StreamPosition)
        {
            return;
        }

        throw new ArgumentException(
            $"The processed event does not match the current position of the partition. Expected {failingPartitionState.Position}, got {processedEvent.CurrentProcessingPosition}",
            nameof(processedEvent));
    }

    StreamProcessorState UpdateFailingPartition(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retriedAt,
        FailingPartitionState failingPartitionState)
    {
        var failingPartition = new FailingPartitionState(
            Position: processedEvent.CurrentProcessingPosition,
            RetryTime: failedProcessing.Retry ? retriedAt.Add(failedProcessing.RetryTimeout) : DateTimeOffset.MaxValue,
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

    public StreamProcessorState WithSuccessfullyProcessed(StreamEvent processedEvent, DateTimeOffset timestamp)
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
                RetryTime = timestamp,
                ProcessingAttempts = 0,
                Reason = "behind"
            };
            return this with
            {
                FailingPartitions = FailingPartitions.SetItem(processedEvent.Partition, failingPartition), LastSuccessfullyProcessed = timestamp
            };
        }

        return this with
        {
            Position = processedEvent.NextProcessingPosition,
            LastSuccessfullyProcessed = timestamp
        };
    }

    static StreamProcessorState AddFailingPartitionFor(
        StreamProcessorState oldState,
        ProcessingPosition failedPosition,
        PartitionId partition,
        DateTimeOffset retryTime,
        string reason,
        DateTimeOffset lastFailed)
    {
        var failingPartition = new FailingPartitionState(failedPosition, retryTime, reason, 1, lastFailed);
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
    public StreamProcessorState WithoutFailingPartitions(IEnumerable<PartitionId> noLongerFailingPartitions) =>
        this with
        {
            FailingPartitions = FailingPartitions.RemoveRange(noLongerFailingPartitions)
        };

    public StreamProcessorState WithoutFailingPartition(PartitionId noLongerFailingPartition) =>
        this with
        {
            FailingPartitions = FailingPartitions.Remove(noLongerFailingPartition)
        };

    public StreamProcessorState WithFailingPartition(PartitionId partitionId, FailingPartitionState partitionState) =>
        this with
        {
            FailingPartitions = FailingPartitions.SetItem(partitionId, partitionState)
        };
}
