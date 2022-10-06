// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing;

/// <summary>
/// Represents a failed <see cref="IProcessingResult" />.
/// </summary>
public class FailedProcessing : IProcessingResult
{
    public static FailedProcessing SingleEvent(string reason,  bool retry, TimeSpan retryTimeout)
        => new(reason, 0, retry, retryTimeout);
    
    public static FailedProcessing Batch(string reason, int batchPosition, bool retry, TimeSpan retryTimeout)
        => new(reason, batchPosition,  retry, retryTimeout);

    FailedProcessing(string reason, int batchPosition, bool retry, TimeSpan retryTimeout)
    {
        FailureReason = reason;
        BatchPosition = batchPosition;
        FailureReason = reason;
        Retry = retry;
        RetryTimeout = retryTimeout;
    }

    /// <inheritdoc/>
    public bool Succeeded { get; }

    /// <inheritdoc/>
    public string FailureReason { get; }

    /// <inheritdoc/>
    public bool Retry { get; }

    /// <inheritdoc/>
    public TimeSpan RetryTimeout { get; }
    
    /// <summary>
    /// Gets the position in the batch that failed processing.
    /// </summary>
    public int BatchPosition { get; }
}
