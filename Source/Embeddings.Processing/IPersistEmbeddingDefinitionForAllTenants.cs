// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a system that can persist an embedding definition for all registered tenants.
    /// </summary>
    public interface IPersistEmbeddingDefinitionForAllTenants
    {
        /// <summary>
        /// Persist the given <see cref="EmbeddingDefinition" /> for all tenants.
        /// </summary>
        /// <param name="definition">The <see cref="EmbeddingDefinition" /> to persist.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether the <see cref="EmbeddingDefinition" /> has been persisted for all tenants.</returns>
        Task<Try> TryPersist(EmbeddingDefinition definition, CancellationToken token);
    }
}
