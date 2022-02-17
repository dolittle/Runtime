// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Projections.Store;

using Dolittle.Runtime.Rudimentary;
using DolittleExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using IExecutionContextManager = Dolittle.Runtime.Execution.IExecutionContextManager;

namespace Dolittle.Runtime.Embeddings.Store.Services;

/// <summary>
/// Represents the implementation of <see cref="IEmbeddingsService" />.
/// </summary>
[Singleton]
public class EmbeddingsService : IEmbeddingsService
{
    readonly Func<IEmbeddingStore> _getEmbeddingStore;
    readonly IExecutionContextManager _executionContextManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingsService"/> class.
    /// </summary>
    /// <param name="getEmbeddingStore"><see cref="Func{T}"/><see cref="IEmbeddingStore" />.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager" />.</param>
    public EmbeddingsService(
        Func<IEmbeddingStore> getEmbeddingStore,
        IExecutionContextManager executionContextManager)
    {
        _getEmbeddingStore = getEmbeddingStore;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    public Task<Try<EmbeddingCurrentState>> TryGetOne(EmbeddingId embedding, ProjectionKey key, DolittleExecutionContext context, CancellationToken token)
    {
        _executionContextManager.CurrentFor(context);
        return _getEmbeddingStore().TryGet(embedding, key, token);
    }

    /// <inheritdoc/>
    public Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, DolittleExecutionContext context, CancellationToken token)
    {
        _executionContextManager.CurrentFor(context);
        return _getEmbeddingStore().TryGetAll(embedding, token);
    }

    /// <inheritdoc/>
    public Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, DolittleExecutionContext context, CancellationToken token)
    {
        _executionContextManager.CurrentFor(context);
        return _getEmbeddingStore().TryGetKeys(embedding, token);
    }
}
