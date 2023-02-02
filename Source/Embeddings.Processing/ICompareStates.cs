// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Defines a system that can compare instances <see cref="ProjectionState"/>.
/// </summary>
public interface ICompareStates
{
    /// <summary>
    /// Tries to compare two projection states to determine equality. Doesn't care about the ordering of
    /// properties or dictionary keys, but will fail on differently ordered arrays.
    /// </summary>
    /// <param name="left">The first <see cref="ProjectionState"/> to compare.</param>
    /// <param name="right">The second <see cref="ProjectionState"/> to compare.</param>
    /// <returns>A <see cref="Try{bool}" /> of true if they are equal, false if not.</returns>
    Try<bool> TryCheckEquality(ProjectionState left, ProjectionState right);
}
