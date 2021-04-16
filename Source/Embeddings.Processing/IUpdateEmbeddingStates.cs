// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a system that can update all embedding states.
    /// </summary>
    public interface IUpdateEmbeddingStates
    {
        /// <summary>
        /// Try to update all the embedding states by projecting all aggregate root events for the given embedding.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> to update states for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A<see cref="Task" /> that, when resolved, returns a<see cref= "Try" /> that indicates whether the operation was successful or not.</returns>
        Task<Try> TryUpdateAllFor(EmbeddingId embedding, CancellationToken cancellationToken);
    }
}