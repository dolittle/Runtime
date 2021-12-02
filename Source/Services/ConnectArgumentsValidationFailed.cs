// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Exception that gets thrown when connect arguments validation failed.
/// </summary>
public class ConnectArgumentsValidationFailed : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="ConnectArgumentsValidationFailed" /> class.
    /// </summary>
    /// <param name="failureReason">The reason why the validation failed.</param>
    public ConnectArgumentsValidationFailed(string failureReason)
        : base($"Reverse call connect arguments validation failed. {failureReason}")
    {
    }
}