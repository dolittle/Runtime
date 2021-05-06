// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{

    /// <summary>
    /// Defines a system, that can detect when an embedding is looping.
    /// </summary>
    public interface IDetectEmdbeddingLoop
    {
        /// <summary>
        /// Try to check if a projection's state is equal to one of it's previous states.
        /// </summary>
        /// <param name="previousStates">An <see cref="IEnumerable{ProjectionState}"/> of the previous states.</param>
        /// <param name="currentState">The current <see cref="ProjectionState"/>.</param>
        /// <returns>A <see cref= "Try" /> that indicates whether the operation was successful or not.</returns>
        Try<bool> TryCheckForProjectionStateLoop(IEnumerable<ProjectionState> previousStates, ProjectionState currentState);
    }
}
