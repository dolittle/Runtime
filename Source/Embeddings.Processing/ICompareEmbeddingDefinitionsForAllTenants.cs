// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store.Definition;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Defines a system that can compare a embedding definition from an embedding registration with the persisted definition.
/// </summary>
public interface ICompareEmbeddingDefinitionsForAllTenants
{
    /// <summary>
    /// Checks whether the given <see cref="EmbeddingDefinition" /> differs from the persisted definition.
    /// </summary>
    /// <param name="definition">The <see cref="EmbeddingDefinition" /> to compare.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether the <see cref="EmbeddingDefinition" /> differs from the persisted definition.</returns>
    Task<IDictionary<TenantId, EmbeddingDefinitionComparisonResult>> DiffersFromPersisted(EmbeddingDefinition definition, CancellationToken token);
}