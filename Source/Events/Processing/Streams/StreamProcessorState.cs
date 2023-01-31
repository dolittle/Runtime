// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents the state of an <see cref="ScopedStreamProcessor" />.
/// </summary>
/// <param name="Position">The <see cref="StreamPosition"/>position of the stream.</param>
/// <param name="FailureReason">The reason for failing.</param>
/// <param name="RetryTime">The <see cref="DateTimeOffset" /> for when to retry processing.</param>
/// <param name="ProcessingAttempts">The number of times it has processed the Event at <see cref="Position" />.</param>
/// <param name="LastSuccessfullyProcessed">Timestamp of last successfull Stream process.</param>
/// <param name="IsFailing">Whether the stream processor is failing.</param>
public record StreamProcessorState(StreamPosition Position, string FailureReason, DateTimeOffset RetryTime, uint ProcessingAttempts, DateTimeOffset LastSuccessfullyProcessed, bool IsFailing) : IStreamProcessorState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
    /// </summary>
    /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
    /// <param name="lastSuccessfullyProcessed">Timestamp of last successfull Stream process.</param>
    public StreamProcessorState(StreamPosition streamPosition, DateTimeOffset lastSuccessfullyProcessed) : this(streamPosition, string.Empty, DateTimeOffset.UtcNow, 0, lastSuccessfullyProcessed, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
    /// </summary>
    /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
    StreamProcessorState(StreamPosition streamPosition) : this(streamPosition, string.Empty, DateTimeOffset.UtcNow, 0, DateTimeOffset.MinValue, false)
    {
    }

    /// <inheritdoc/>
    public bool Partitioned => false;

    public static StreamProcessorState New => new(StreamPosition.Start);
}