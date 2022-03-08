// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a the result of a projection operation that resulted in a replace.
/// </summary>
public class ProjectionReplaceResult : SuccessfulProjectionResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionReplaceResult"/> class.
    /// </summary>
    /// <param name="state">The new <see cref="ProjectionState"/> to persist.</param>
    public ProjectionReplaceResult(ProjectionState state)
    {
        State = state;
    }

    /// <inheritdoc />
    public ProjectionState State { get; }
}
