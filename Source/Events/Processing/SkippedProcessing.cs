// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing;

/// <summary>
/// Represents a skipped event in the stream, used if the event partition is failing <see cref="IProcessingResult" />.
/// </summary>
public class SkippedProcessing : SuccessfulProcessing
{
    public static readonly SkippedProcessing Instance = new();
    
    /// <inheritdoc/>
    public bool Succeeded => true;

    /// <inheritdoc/>
    public string FailureReason => string.Empty;

    /// <inheritdoc/>
    public bool Retry => false;

    /// <inheritdoc/>
    public TimeSpan RetryTimeout => TimeSpan.Zero;
}
