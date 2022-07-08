// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Projections.Store.Services;

/// <summary>
/// Represents the implementation of <see cref="IProjectionsService" />.
/// </summary>
[Singleton]
public class ProjectionsService : IProjectionsService
{
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly Func<TenantId, IProjectionStore> _getProjectionStoreFor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsService"/> class.
    /// </summary>
    /// <param name="executionContextCreator">The execution context creator to use to validate execution contexts.</param>
    /// <param name="getProjectionStoreFor">The factory to use to create projection stores per tenant.</param>
    public ProjectionsService(
        ICreateExecutionContexts executionContextCreator,
        Func<TenantId, IProjectionStore> getProjectionStoreFor)
    {
        _executionContextCreator = executionContextCreator;
        _getProjectionStoreFor = getProjectionStoreFor;
    }

    /// <inheritdoc/>
    public Task<Try<ProjectionCurrentState>> TryGetOne(ProjectionId projection, ScopeId scope, ProjectionKey key, ExecutionContext context, CancellationToken token)
        => _executionContextCreator
            .TryCreateUsing(context)
            .Then(_ => _getProjectionStoreFor(_.Tenant))
            .Then(_ => _.TryGet(projection, scope, key, token));

    /// <inheritdoc/>
    public Task<Try<IAsyncEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, ExecutionContext context, CancellationToken token)
        => _executionContextCreator
            .TryCreateUsing(context)
            .Then(_ => _getProjectionStoreFor(_.Tenant))
            .Then(_ => _.TryGetAll(projection, scope, token));
}
