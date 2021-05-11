// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IDetectEmbeddingLoop" />.
    /// </summary>
    public class EmbeddingLoopDetector : IDetectEmdbeddingLoop
    {
        readonly ICompareStates _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingLoopDetector"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="ICompareStates"/> to use for comparing current and previous states.</param>
        public EmbeddingLoopDetector(ICompareStates comparer)
        {
            _comparer = comparer;
        }

        /// <inheritdoc/>
        public Try<bool> TryCheckForProjectionStateLoop(ProjectionState currentState, IEnumerable<ProjectionState> previousStates)
        {
            var equalityResults = previousStates.AsParallel()
                .Select(previousState => _comparer.TryCheckEquality(previousState, currentState))
                // The query execution is deferred so need to call for ToList here
                // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq#the-forall-operator
                .ToList();
            var failure = equalityResults.FirstOrDefault(_ => !_.Success);
            if (failure is not null)
            {
                return failure.Exception;
            }
            return equalityResults.Any(_ => _.Result);
        }
    }
}
