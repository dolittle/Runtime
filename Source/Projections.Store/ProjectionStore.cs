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
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionStore"/> class.
    /// </summary>
    /// <param name="projectionStates">The <see cref="IProjectionStates"/> to use for getting projection states.</param>
    /// <param name="projectionDefinitions">The <see cref="IProjectionStates"/> to use for getting initial projection states.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ProjectionStore(
        IProjectionStates projectionStates,
        IProjectionDefinitions projectionDefinitions,
        ILogger logger
    )
    {
        _projectionStates = projectionStates;
        _projectionDefinitions = projectionDefinitions;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Try<ProjectionCurrentState>> TryGet(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
    {
        try
        {
            Log.GettingOneProjection(_logger, projection, scope, key);

            var state = await _projectionStates.TryGet(projection, scope, key, token).ConfigureAwait(false);

            return state switch
            {
                { Success: true } => new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, state.Result, key),
                { Success: false, Exception: ProjectionStateDoesNotExist } => await TryGetInitialState(projection, key, scope, token).ConfigureAwait(false),
                _ => state.Exception
            };
        }
        catch (Exception ex)
        {
            Log.ErrorGettingOneProjection(_logger, ex);
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<Try<IAsyncEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, CancellationToken token)
    {
        try
        {
            Log.GettingAllProjections(_logger, projection, scope);

            var tryGetStates = await _projectionStates.TryGetAll(projection, scope, token).ConfigureAwait(false);
            return tryGetStates.Select(_ => _.Select(_ => new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, _.State, _.Key)));
        }
        catch (Exception ex)
        {
            Log.ErrorGettingAllProjections(_logger, ex);
            return ex;
        }
    }

    async Task<Try<ProjectionCurrentState>> TryGetInitialState(ProjectionId projection, ProjectionKey key, ScopeId scope, CancellationToken token)
    {
        var definition = await _projectionDefinitions.TryGet(projection, scope, token).ConfigureAwait(false);
        if (!definition.Success)
        {
            return definition.Exception;
        }

        return new ProjectionCurrentState(ProjectionCurrentStateType.CreatedFromInitialState, definition.Result.InitialState, key);
    }

    /// <inheritdoc/>
    public Task<bool> TryReplace(ProjectionId projection, ScopeId scope, ProjectionKey key, ProjectionState state, CancellationToken token)
        => _projectionStates.TryReplace(projection, scope, key, state, token);

    /// <inheritdoc/>
    public Task<bool> TryRemove(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
        => _projectionStates.TryRemove(projection, scope, key, token);

    /// <inheritdoc/>
    public Task<bool> TryDrop(ProjectionId projection, ScopeId scope, CancellationToken token)
        => _projectionStates.TryDrop(projection, scope, token);
}
