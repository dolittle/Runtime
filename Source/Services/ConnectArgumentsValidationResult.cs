// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents the validation result from validating the reverse call connect arguments.
/// </summary>
public record ConnectArgumentsValidationResult
{
    /// <summary>
    /// Creates a successful <see cref="ConnectArgumentsValidationResult" />.
    /// </summary>
    public static ConnectArgumentsValidationResult Ok { get; } = new() { Success = true, FailureReason = default };

    /// <summary>
    /// Creates a failed <see cref="ConnectArgumentsValidationResult" />.
    /// </summary>
    /// <param name="failureReason">The reason why validation failed.</param>
    public static ConnectArgumentsValidationResult Failed(string failureReason) => new() { Success = false, FailureReason = failureReason };

    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool Success { get; private init; }

    /// <summary>
    /// Gets the reason for why the validation failed.
    /// </summary>
    public string? FailureReason { get; private init; }
}
