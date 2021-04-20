// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a system that manages all instances of <see cref="IEmbeddingProcessor"/>.
    /// </summary>
    public interface IEmbeddingProcessors
    {
        /// <summary>
        /// Starts an instance of an <see cref="IEmbeddingProcessor"/> for all tenants for a given embedding.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> to start processors for.</param>
        /// <param name="factory">A <see cref="EmbeddingProcessorFactory"/> to use for creating instances of <see cref="IEmbeddingProcessor"/> for each tenant.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to signal the started processors to stop.</param>
        /// <returns>A <see cref="Task"/> that is resolved when one or all of the started <see cref="IEmbeddingProcessor"/> complete or fail.</returns>
        Task<Try> TryStartEmbeddingProcessorForAllTenants(EmbeddingId embedding, EmbeddingProcessorFactory factory, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if there are any instances of <see cref="IEmbeddingProcessor"/> running for the given embedding.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> to check for.</param>
        /// <returns>true if there are processors running, false if not.</returns>
        bool HasEmbeddingProcessors(EmbeddingId embedding);

        /// <summary>
        /// Get a specific <see cref="IEmbeddingProcessor"/> for a given tenant and embedding.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId"/> to get the processor for.</param>
        /// <param name="embedding">The <see cref="EmbeddingId"/> to get the processor for.</param>
        /// <param name="processor">The running <see cref="IEmbeddingProcessor"/> if it exists, null if not.</param>
        /// <returns>true if the processors was found, false if not.</returns>
        bool TryGetEmbeddingProcessorFor(TenantId tenant, EmbeddingId embedding, out IEmbeddingProcessor processor);
    }
}