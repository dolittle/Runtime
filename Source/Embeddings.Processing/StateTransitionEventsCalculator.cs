// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ICalculateStateTransitionEvents" />.
    /// </summary>
    public class StateTransitionEventsCalculator : ICalculateStateTransitionEvents
    {
        readonly EmbeddingId _identifier;
        readonly IEmbedding _embedding;

        /// <summary>
        /// Initializes an instance of the <see cref="StateTransitionEventsCalculator" /> class.
        /// </summary>
        /// <param name="identifier">The <see cref="EmbeddingId" />.</param>
        /// <param name="embedding">The <see cref="IEmbedding" />.</param>
        public StateTransitionEventsCalculator(
            EmbeddingId identifier,
            IEmbedding embedding
        )
        {
            _identifier = identifier;
            _embedding = embedding;
        }

        /// <inheritdoc/>
        public Task<Try<UncommittedAggregateEvents>> TryConverge(EmbeddingCurrentState current, ProjectionState desired, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<Try<UncommittedAggregateEvents>> TryDelete(EmbeddingCurrentState current, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}