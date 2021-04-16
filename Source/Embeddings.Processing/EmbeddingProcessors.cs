// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingProcessors"/>.
    /// </summary>
    public class EmbeddingProcessors : IEmbeddingProcessors
    {
        readonly ITenants _tenants;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingProcessors"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants"/> to use to get all configured tenants.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public EmbeddingProcessors(ITenants tenants, ILogger logger)
        {
            _tenants = tenants;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task StartEmbeddingProcessorForAllTenants(EmbeddingId embedding, EmbeddingProcessorFactory factory, CancellationToken cancellationToken) => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public bool HasEmbeddingProcessors(EmbeddingId embedding) => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public bool TryGetEmbeddingProcessorFor(TenantId tenant, EmbeddingId embedding, out IEmbeddingProcessor processor) => throw new System.NotImplementedException();
    }
}