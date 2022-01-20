// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using DolittleExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using IExecutionContextManager = Dolittle.Runtime.Execution.IExecutionContextManager;

namespace Dolittle.Runtime.Projections.Store.Services;

/// <summary>
/// Represents the implementation of <see cref="IProjectionsService" />.
/// </summary>
[Singleton]
public class ProjectionsService : IProjectionsService
{
    readonly FactoryFor<IProjectionStore> _getProjectionStore;
    readonly IExecutionContextManager _executionContextManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreService"/> class.
    /// </summary>
    /// <param name="getProjectionStore"><see cref="FactoryFor{T}"/><see cref="IProjectionStore" />.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager" />.</param>
    public ProjectionsService(
        FactoryFor<IProjectionStore> getProjectionStore,
        IExecutionContextManager executionContextManager
    )
    {
        _getProjectionStore = getProjectionStore;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    public Task<Try<ProjectionCurrentState>> TryGetOne(ProjectionId projection, ScopeId scope, ProjectionKey key, DolittleExecutionContext context, CancellationToken token)
    {
        _executionContextManager.CurrentFor(context);
        return _getProjectionStore().TryGet(projection, scope, key, token);
    }

    /// <inheritdoc/>
    public Task<Try<IEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, DolittleExecutionContext context, CancellationToken token)
    {
        _executionContextManager.CurrentFor(context);
        return _getProjectionStore().TryGetAll(projection, scope, token);
    }
}