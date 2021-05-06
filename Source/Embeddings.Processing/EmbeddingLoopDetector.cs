// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <inheritdoc/>
    public class EmbeddingLoopDetector : IDetectEmdbeddingLoop
    {
        readonly ICompareStates _comparer = new CompareProjectionStates();

        /// <inheritdoc/>
        public Try<bool> TryCheckForProjectionStateLoop(IEnumerable<ProjectionState> previousStates, ProjectionState currentState)
        {
            try
            {
                return previousStates.All(previousState =>
                {
                    return !_comparer.TryCheckEquality(previousState, currentState).Result;
                });
            }
            catch (Exception ex)
            {
                return ex;
            }

        }
    }
}
