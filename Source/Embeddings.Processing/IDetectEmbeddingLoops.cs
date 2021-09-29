// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a system that can detect loops in the logic of an <see cref="IEmbedding"/>.
    /// </summary>
    public interface IDetectEmbeddingLoops
    {
        /// <summary>
        /// Try to check if a projection's state is equal to one of it's previous states.
        /// </summary>
        /// <param name="currentState">The current <see cref="ProjectionState"/>.</param>
        /// <param name="previousStates">An <see cref="IEnumerable{ProjectionState}"/> of the previous states.</param>
        /// <returns>A <see cref= "Try" /> that indicates whether the operation was successful or not.</returns>
        Try<bool> TryCheckForProjectionStateLoop(ProjectionState currentState, IEnumerable<ProjectionState> previousStates);
    }
}
