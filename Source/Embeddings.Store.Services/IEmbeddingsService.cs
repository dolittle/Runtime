// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using RuntimeExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Store.Services
{
    /// <summary>
    /// Defines the service that retrieves Embeddings.
    /// </summary>
    public interface IEmbeddingsService
    {
        /// <summary>
        /// Try to get an embedding state.
        /// </summary>
        /// <param name="embedding"><see cref="EmbeddingId" /> to get state from.</param>
        /// <param name="key"><see cref="ProjectionKey" /> of the state to get.</param>
        /// <param name="context"><see cref="RuntimeExecutionContext" /> the embedding should be gotten in.</param>
        /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="EmbeddingCurrentState" />.</returns>
        Task<Try<EmbeddingCurrentState>> TryGetOne(EmbeddingId embedding, ProjectionKey key, RuntimeExecutionContext context, CancellationToken token);

        /// <summary>
        /// Try to get all the embedding states.
        /// </summary>
        /// <param name="embedding"><see cref="EmbeddingId" /> to get state from.</param>
        /// <param name="context"><see cref="RuntimeExecutionContext" /> the embeddings should be gotten in.</param>
        /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="IEnumerable{T}" /> of <see cref="EmbeddingCurrentState" />.</returns>
        Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, RuntimeExecutionContext context, CancellationToken token);

        /// <summary>
        /// Try to get all the keys of an embedding.
        /// </summary>
        /// <param name="embedding"><see cref="EmbeddingId" /> to get state from.</param>
        /// <param name="context"><see cref="RuntimeExecutionContext" /> the embedding keys should be gotten in.</param>
        /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="IEnumerable{T}" /> of <see cref="ProjectionKey" />.</returns>
        Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, RuntimeExecutionContext context, CancellationToken token);
    }
}
