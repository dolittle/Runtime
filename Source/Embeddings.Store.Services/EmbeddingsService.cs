// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Store.Services;

/// <summary>
/// Represents the implementation of <see cref="IEmbeddingsService" />.
/// </summary>
[Singleton]
public class EmbeddingsService : IEmbeddingsService
{
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly Func<TenantId, IEmbeddingStore> _getEmbeddingStoreFor;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingsService"/> class.
    /// </summary>
    /// <param name="executionContextCreator">The execution context creator to use to validate execution contexts.</param>
    /// <param name="getEmbeddingStoreFor">The factory to use to create embedding stores per tenant.</param>
    public EmbeddingsService(
        ICreateExecutionContexts executionContextCreator,
        Func<TenantId, IEmbeddingStore> getEmbeddingStoreFor)
    {
        _executionContextCreator = executionContextCreator;
        _getEmbeddingStoreFor = getEmbeddingStoreFor;
    }

    /// <inheritdoc/>
    public Task<Try<EmbeddingCurrentState>> TryGetOne(EmbeddingId embedding, ProjectionKey key, ExecutionContext context, CancellationToken token)
        => _executionContextCreator
            .TryCreateUsing(context)
            .Then(_ => _getEmbeddingStoreFor(_.Tenant))
            .Then(_ => _.TryGet(embedding, key, token));

    /// <inheritdoc/>
    public Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, ExecutionContext context, CancellationToken token)
        => _executionContextCreator
            .TryCreateUsing(context)
            .Then(_ => _getEmbeddingStoreFor(_.Tenant))
            .Then(_ => _.TryGetAll(embedding, token));

    /// <inheritdoc/>
    public Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, ExecutionContext context, CancellationToken token)
        => _executionContextCreator
            .TryCreateUsing(context)
            .Then(_ => _getEmbeddingStoreFor(_.Tenant))
            .Then(_ => _.TryGetKeys(embedding, token));
}
