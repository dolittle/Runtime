// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Exception that gets thrown when attempting to create an <see cref="ExecutionContext"/> that contains fields that conflict with the current Runtime configuration.
/// </summary>
public class InvalidExecutionContext : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidExecutionContext"/> class.
    /// </summary>
    /// <param name="property">The execution context property that conflicts with the configured value.</param>
    /// <param name="configuredValue">The configured value of the property.</param>
    /// <param name="requestedValue">The requested value of the property.</param>
    public InvalidExecutionContext(string property, string configuredValue, string requestedValue)
        : base($"The requested {nameof(ExecutionContext)} is not valid because {property} conflicts with the configured value. Expected {configuredValue}, requested {requestedValue}")
    {
    }
}
