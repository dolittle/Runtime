// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a the result of a projection operation that resulted in a delete.
/// </summary>
public abstract class SuccessfulProjectionResult : IProjectionResult
{
    public bool Succeeded => true;

    public string FailureReason { get; }

    public bool Retry => false;

    public TimeSpan RetryTimeout { get; }
}
