// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines an embedding.
    /// </summary>
    public interface IEmbedding : IProjection
    {
        /// <summary>
        /// Tries to get <see cref="UncommittedEvents"/> that will get the current state closer to the desired state.
        /// </summary>
        /// <param name="currentState">The <see cref="EmbeddingCurrentState">current state</see>.</param>
        /// <param name="desiredState">The <see cref="ProjectionState">desired state</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="UncommittedEvents" />.</returns>
        Task<Try<UncommittedEvents>> TryCompare(EmbeddingCurrentState currentState, ProjectionState desiredState, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to get <see cref=" UncommittedEvents"/> that will get the current state closer to be deleted.
        /// </summary>
        /// <param name="currentState">The <see cref="EmbeddingCurrentState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="UncommittedEvents" />.</returns>
        Task<Try<UncommittedEvents>> TryDelete(EmbeddingCurrentState currentState, CancellationToken cancellationToken);
    }
}