// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Embeddings.Store.Services.Grpc
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly EmbeddingStoreGrpcService _embeddingStoreGrpcService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="embeddingStore">The <see cref="EmbeddingsService"/>.</param>
        public RuntimeServices(EmbeddingStoreGrpcService embeddingStore)
        {
            _embeddingStoreGrpcService = embeddingStore;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Embeddings";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new Service[]
            {
                new(_embeddingStoreGrpcService, Contracts.EmbeddingStore.BindService(_embeddingStoreGrpcService), Contracts.EmbeddingStore.Descriptor)
            };
    }
}
