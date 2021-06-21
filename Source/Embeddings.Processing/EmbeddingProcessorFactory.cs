// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Embeddings.Processing
{
    [Singleton]
    public class EmbeddingProcessorFactory : IEmbeddingProcessorFactory
    {
        /// <inheritdoc/>
        public IEmbeddingProcessor Create(TenantId tenant, EmbeddingId embedding)
        {
            throw new NotImplementedException();
        }
    }
}
