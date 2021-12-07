// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a the result of a projection operation that failed.
/// </summary>
public class ProjectionFailedResult : IProjectionResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionFailedResult"/> class.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> that caused the failure.</param>
    public ProjectionFailedResult(Exception exception)
    {
        Exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionFailedResult"/> class.
    /// </summary>
    /// <param name="reason">The reason for the failure.</param>
    public ProjectionFailedResult(string reason)
        : this(new ProjectionFailed(reason))
    {
    }

    public Exception Exception { get; }
}