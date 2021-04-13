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

namespace Dolittle.Runtime.Projections.Store
{
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
                _logger.GettingOneProjection(projection, scope, key);

                var tryGetState = await _projectionStates.TryGet(projection, scope, key, token).ConfigureAwait(false);
                if (tryGetState.Success) return new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, tryGetState.Result, key);
                if (tryGetState.HasException) return tryGetState.Exception;

                var tryGetDefinition = await _projectionDefinitions.TryGet(projection, scope, token).ConfigureAwait(false);
                if (tryGetDefinition.Success) return new ProjectionCurrentState(ProjectionCurrentStateType.CreatedFromInitialState, tryGetDefinition.Result.InititalState, key);
                if (tryGetDefinition.HasException) return tryGetDefinition.Exception;

                return new FailedToGetProjectionDefinition(projection, scope);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting projection");
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, CancellationToken token)
        {
            try
            {
                _logger.GettingAllProjections(projection, scope);

                var tryGetStates = await _projectionStates.TryGetAll(projection, scope, token).ConfigureAwait(false);
                if (tryGetStates.Success) return new Try<IEnumerable<ProjectionCurrentState>>(true, tryGetStates.Result.Select(_ => new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, _.State, _.Key)));
                if (tryGetStates.HasException) return tryGetStates.Exception;
                return Array.Empty<ProjectionCurrentState>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting all projections");
                return ex;
            }
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
}