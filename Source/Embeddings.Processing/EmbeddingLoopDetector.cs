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
        public Try<bool> TryCheckForProjectionStateLoop(ProjectionState currentState, IEnumerable<ProjectionState> previousStates)
        {
            try
            {
                return previousStates.AsParallel().Any(previousState =>
                {
                    var tryResult = _comparer.TryCheckEquality(previousState, currentState);
                    return tryResult.Success ? tryResult.Result : throw tryResult.Exception;
                });
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
