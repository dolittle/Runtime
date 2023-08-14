// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Actors;

namespace Dolittle.Runtime.Events.Store.Streams;

public interface IStreamProcessorState
{
    /// <summary>
    /// Gets the <see cref="ProcessingPosition" />.
    /// </summary>
    ProcessingPosition Position { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="AbstractScopedStreamProcessor" /> is partitioned or not.
    /// </summary>
    bool Partitioned { get; }
    
    /// <summary>
    /// Check if the processor has recorded any failures.
    /// </summary>
    int FailingPartitionCount { get; }

    Bucket ToProtobuf();

    /// <summary>
    ///  Get the earliest position that the stream processor needs to process. This is normally the same as the <see cref="Position"/>
    /// but can be earlier in the stream if the stream processor is partitioned and there exists failing partitions.
    /// </summary>
    ProcessingPosition EarliestProcessingPosition { get; }

    public bool TryGetTimespanToRetry(out TimeSpan timeToRetry);

    public IStreamProcessorState WithResult(IProcessingResult result, StreamEvent processedEvent, DateTimeOffset timestamp);
    public IStreamProcessorState WithFailure(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retryAt, DateTimeOffset timestamp);
    public IStreamProcessorState WithSuccessfullyProcessed(StreamEvent processedEvent, DateTimeOffset timestamp);
}

/// <summary>
/// Defines the basis for the state of a <see cref="AbstractScopedStreamProcessor" />.
/// </summary>
public interface IStreamProcessorState<T> : IStreamProcessorState where T : IStreamProcessorState<T>
{
    IStreamProcessorState IStreamProcessorState.WithResult(IProcessingResult result, StreamEvent processedEvent, DateTimeOffset timestamp) =>
        WithResult(result, processedEvent, timestamp);

    IStreamProcessorState IStreamProcessorState.WithFailure(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retryAt, DateTimeOffset timestamp) =>
        WithFailure(failedProcessing, processedEvent, retryAt, timestamp);

    IStreamProcessorState IStreamProcessorState.WithSuccessfullyProcessed(StreamEvent processedEvent, DateTimeOffset timestamp) =>
        WithSuccessfullyProcessed(processedEvent, timestamp);

    public new T WithResult(IProcessingResult result, StreamEvent processedEvent, DateTimeOffset timestamp);

    public new T WithFailure(IProcessingResult failedProcessing, StreamEvent processedEvent, DateTimeOffset retryAt, DateTimeOffset timestamp);
    public new T WithSuccessfullyProcessed(StreamEvent processedEvent, DateTimeOffset timestamp);
}
