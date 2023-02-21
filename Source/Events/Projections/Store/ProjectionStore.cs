// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Represents the implementation of <see cref="IProjectionStore"/>
/// </summary>
public class ProjectionStore : IProjectionStore
{
    readonly IProjectionStates _projectionStates;
    readonly IProjectionDefinitions _projectionDefinitions;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStore"/> class.
    /// </summary>
    /// <param name="projectionStates">The <see cref="IProjectionStates"/> to use for getting projection states.</param>
    /// <param name="projectionDefinitions">The <see cref="IProjectionStates"/> to use for getting initial projection states.</param>
    /// <param name="metrics">The metrics collector to use.</param>
    /// <param name="logger">The logger to use.</param>
    public ProjectionStore(
        IProjectionStates projectionStates,
        IProjectionDefinitions projectionDefinitions,
        IMetricsCollector metrics,
        ILogger logger
    )
    {
        _projectionStates = projectionStates;
        _projectionDefinitions = projectionDefinitions;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Try<ProjectionCurrentState>> TryGet(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
    {
        try
        {
            Log.GettingOneProjection(_logger, projection, scope, key);
            _metrics.IncrementTotalGet();

            var state = await _projectionStates.TryGet(projection, scope, key, token).ConfigureAwait(false);

            switch (state) 
            {
                case { Success: true }:
                    return new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, state.Result, key);
                case { Success: false, Exception: ProjectionStateDoesNotExist }:
                    return await TryGetInitialState(projection, key, scope, token).ConfigureAwait(false);
                default:
                    _metrics.IncrementTotalFailedGet();
                    return state.Exception;
            };
        }
        catch (Exception ex)
        {
            Log.ErrorGettingOneProjection(_logger, ex);
            _metrics.IncrementTotalFailedGet();
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<Try<IAsyncEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, CancellationToken token)
    {
        try
        {
            Log.GettingAllProjections(_logger, projection, scope);
            _metrics.IncrementTotalGetAll();

            var states = await _projectionStates.TryGetAll(projection, scope, token).ConfigureAwait(false);

            if (!states.Success)
            {
                _metrics.IncrementTotalFailedGetAll();
            }
            return states.Select(_ => _.Select(_ => new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, _.State, _.Key)));
        }
        catch (Exception ex)
        {
            Log.ErrorGettingAllProjections(_logger, ex);
            _metrics.IncrementTotalFailedGetAll();
            return ex;
        }
    }

    async Task<Try<ProjectionCurrentState>> TryGetInitialState(ProjectionId projection, ProjectionKey key, ScopeId scope, CancellationToken token)
    {
        var definition = await _projectionDefinitions.TryGet(projection, scope, token).ConfigureAwait(false);
        if (!definition.Success)
        {
            _metrics.IncrementTotalFailedGet();
            return definition.Exception;
        }

        return new ProjectionCurrentState(ProjectionCurrentStateType.CreatedFromInitialState, definition.Result.InitialState, key);
    }

    /// <inheritdoc/>
    public async Task<bool> TryReplace(ProjectionId projection, ScopeId scope, ProjectionKey key, ProjectionState state, CancellationToken token)
    {
        _metrics.IncrementTotalProjectionStoreReplacements();
       
        var result = await _projectionStates.TryReplace(projection, scope, key, state, token).ConfigureAwait(false);
        if (!result)
        {
            _metrics.IncrementTotalFailedProjectionStoreReplacements();
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> TryRemove(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
    {
        _metrics.IncrementTotalProjectionStoreRemovals();

        var result = await _projectionStates.TryRemove(projection, scope, key, token).ConfigureAwait(false);
        if (!result)
        {
            _metrics.IncrementTotalFailedProjectionStoreRemovals();
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> TryDrop(ProjectionId projection, ScopeId scope, CancellationToken token)
    {
        _metrics.IncrementTotalProjectionStoreDrops();

        var result = await _projectionStates.TryDrop(projection, scope, token).ConfigureAwait(false);
        if (!result)
        {
            _metrics.IncrementTotalFailedProjectionStoreDrops();
        }

        return result;
    }
}
