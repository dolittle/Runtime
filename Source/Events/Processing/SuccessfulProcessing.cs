// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing;

/// <summary>
/// Represents a successful <see cref="IProcessingResult" />.
/// </summary>
public class SuccessfulProcessing : IProcessingResult
{
    /// <inheritdoc/>
    public bool Succeeded => true;

    /// <inheritdoc/>
    public string FailureReason => string.Empty;

    /// <inheritdoc/>
    public bool Retry => false;

    /// <inheritdoc/>
    public TimeSpan RetryTimeout => TimeSpan.Zero;
}