// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using DolittleExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using IExecutionContextManager = Dolittle.Runtime.Execution.IExecutionContextManager;

namespace Dolittle.Runtime.Projections.Store.Services
{
    /// <summary>
    /// Represents the implementation of <see cref="IProjectionsService" />.
    /// </summary>
    [Singleton]
    public class ProjectionsService : IProjectionsService
    {
        readonly FactoryFor<IProjectionStates> _getProjectionStates;
        readonly FactoryFor<IProjectionDefinitions> _getProjectionDefinitions;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreService"/> class.
        /// </summary>
        /// <param name="getProjectionStates"><see cref="FactoryFor{T}"/><see cref="IProjectionStates" />.</param>
        /// <param name="getProjectionDefinitions"><see cref="FactoryFor{T}"/><see cref="IProjectionDefinitions" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ProjectionsService(
            FactoryFor<IProjectionStates> getProjectionStates,
            FactoryFor<IProjectionDefinitions> getProjectionDefinitions,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _getProjectionStates = getProjectionStates;
            _getProjectionDefinitions = getProjectionDefinitions;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, DolittleExecutionContext context, CancellationToken token)
        {
            try
            {
                _executionContextManager.CurrentFor(context);
                _logger.GettingAllProjections(projection, scope);
                var tryGetStates = await _getProjectionStates().TryGetAll(projection, scope, token).ConfigureAwait(false);
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
        public async Task<Try<ProjectionCurrentState>> TryGetOne(ProjectionId projection, ScopeId scope, ProjectionKey key, DolittleExecutionContext context, CancellationToken token)
        {
            try
            {
                _executionContextManager.CurrentFor(context);
                _logger.GettingAllProjections(projection, scope);
                var tryGetState = await _getProjectionStates().TryGet(projection, scope, key, token).ConfigureAwait(false);
                if (tryGetState.Success) return new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, tryGetState.Result, key);
                if (tryGetState.HasException) return tryGetState.Exception;
                return await GetInitialState(projection, scope, key, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting projection");
                return ex;
            }
        }

        async Task<Try<ProjectionCurrentState>> GetInitialState(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
        {
            var tryGetDefinition = await _getProjectionDefinitions().TryGet(projection, scope, token).ConfigureAwait(false);
            if (tryGetDefinition.Success) return new ProjectionCurrentState(ProjectionCurrentStateType.CreatedFromInitialState, tryGetDefinition.Result.InititalState, key);
            if (tryGetDefinition.HasException) return tryGetDefinition.Exception;
            return new FailedToGetProjectionDefinition(projection, scope);
        }
    }
}
