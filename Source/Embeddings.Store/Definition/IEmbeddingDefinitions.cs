// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store.Definition;

/// <summary>
/// Defines a repository for <see cref="EmbeddingDefinition">projection definitions</see>.
/// </summary>
public interface IEmbeddingDefinitions
{
    /// <summary>
    /// Try to get a <see cref="EmbeddingDefinition" /> for a projection.
    /// </summary>
    /// <param name="projection">The <see cref="EmbeddingId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> of <see cref="EmbeddingDefinition" />.</returns>
    Task<Try<EmbeddingDefinition>> TryGet(EmbeddingId projection, CancellationToken token);

    /// <summary>
    /// Try to persist a <see cref="EmbeddingDefinition" />.
    /// </summary>
    /// <param name="definition">The <see cref="EmbeddingDefinition" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether the new state was persisted.</returns>
    Task<Try<bool>> TryPersist(EmbeddingDefinition definition, CancellationToken token);
}