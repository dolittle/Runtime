// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections;

record ProjectionIdentifier(ScopeId Scope, ProjectionId Projection);

/// <summary>
/// Represents an implementation of <see cref="IProjections"/>.
/// </summary>
public class Projections : IProjections
{
    readonly ConcurrentDictionary<ProjectionIdentifier, ProjectionProcessor> _projections = new();
    
    readonly IStreamProcessors _streamProcessors;
    readonly ICompareProjectionDefinitionsForAllTenants _projectionDefinitionComparer;
    readonly FactoryFor<IProjectionPersister> _getProjectionPersister;
    readonly FactoryFor<IProjectionStore> _getProjectionStore;
    readonly FactoryFor<IProjectionDefinitions> _getProjectionDefinitions;
    readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
    readonly IProjectionKeys _projectionKeys;
    readonly IPerformActionOnAllTenants _onAllTenants;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;

    public Projections(
        IStreamProcessors streamProcessors,
        ICompareProjectionDefinitionsForAllTenants projectionDefinitionComparer,
        FactoryFor<IProjectionPersister> getProjectionPersister,
        FactoryFor<IProjectionStore> getProjectionStore,
        FactoryFor<IProjectionDefinitions> getProjectionDefinitions,
        FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
        IProjectionKeys projectionKeys,
        IPerformActionOnAllTenants onAllTenants,
        ILoggerFactory loggerFactory)
    {
        _streamProcessors = streamProcessors;
        _projectionDefinitionComparer = projectionDefinitionComparer;
        _getProjectionPersister = getProjectionPersister;
        _getProjectionStore = getProjectionStore;
        _getProjectionDefinitions = getProjectionDefinitions;
        _getStreamProcessorStates = getStreamProcessorStates;
        _projectionKeys = projectionKeys;
        _onAllTenants = onAllTenants;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<Projections>();
    }

    /// <inheritdoc />
    public IEnumerable<ProjectionDefinition> All => _projections.Values.Select(_ => _.Definition);
    
    /// <inheritdoc />
    public Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(ScopeId scopeId, ProjectionId projectionId) 
        => _projections.TryGetValue(new ProjectionIdentifier(scopeId, projectionId), out var processor)
            ? processor.GetCurrentStates()
            : new ProjectionNotRegistered(scopeId, projectionId);

    /// <inheritdoc />
    public async Task<Try<ProjectionProcessor>> Register(IProjection projection, CancellationToken cancellationToken)
    {
        var identifier = new ProjectionIdentifier(projection.Definition.Scope, projection.Definition.Projection);
        if (_projections.ContainsKey(identifier))
        {
            Log.ProjectionAlreadyRegistered(_logger, identifier.Scope, identifier.Projection);
            return new ProjectionAlreadyRegistered(identifier.Scope, identifier.Projection);
        }
        
        var registration = _streamProcessors.TryCreateAndRegister(
            projection.Definition.Scope,
            projection.Definition.Projection.Value,
            new EventLogStreamDefinition(),
            () => new EventProcessor(
                projection.Definition,
                _getProjectionPersister(),
                _getProjectionStore(),
                _projectionKeys,
                projection,
                _loggerFactory.CreateLogger<EventProcessor>()),
            cancellationToken);

        if (!registration.Success)
        {
            Log.FailedToRegisterProjectionStreamProcessor(_logger, identifier.Scope, identifier.Projection, registration.Exception);
            return registration.Exception;
        }

        var processor = new ProjectionProcessor(
            projection,
            registration.Result,
            () => _projections.TryRemove(identifier, out _));
        
        if (!_projections.TryAdd(identifier, processor))
        {
            Log.ProjectionAlreadyRegistered(_logger, identifier.Scope, identifier.Projection);
            return new ProjectionAlreadyRegistered(identifier.Scope, identifier.Projection);
        }

        var resetting = await ResetProjectionIfDefinitionHasChanged(identifier, projection.Definition, cancellationToken).ConfigureAwait(false);
        if (!resetting.Success)
        {
            processor.Dispose();
            return resetting.Exception;
        }

        return processor;
    }

    /// <inheritdoc />
    public Task<Try> ReplayEventsForTenant(ScopeId scopeId, ProjectionId projectionId, TenantId tenantId) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public Task<Try> ReplayEventsForAllTenants(ScopeId scopeId, ProjectionId projectionId) => throw new System.NotImplementedException();

    async Task<Try> ResetProjectionIfDefinitionHasChanged(ProjectionIdentifier identifier, ProjectionDefinition definition, CancellationToken cancellationToken)
    {
        var tenantsToResetFor = await GetTenantsWhereDefinitionHasChanged(identifier, definition, cancellationToken);
        
        var dropping = await DropStatesAndResetStreamProcessorsFor(tenantsToResetFor, definition, cancellationToken);
        if (!dropping.Success)
        {
            return dropping.Exception;
        }

        var persistence = await PersistDefinitionForAllTenants(definition, cancellationToken);
        if (!persistence.Success)
        {
            return persistence.Exception;
        }
        
        return Try.Succeeded();
    }

    async Task<IEnumerable<TenantId>> GetTenantsWhereDefinitionHasChanged(ProjectionIdentifier identifier, ProjectionDefinition definition, CancellationToken cancellationToken)
    {
        var tenantsToResetFor = new List<TenantId>();
        
        var tenantComparisonResults = await _projectionDefinitionComparer.DiffersFromPersisted(definition, cancellationToken).ConfigureAwait(false);
        foreach (var (tenant, result) in tenantComparisonResults)
        {
            if (result.Succeeded)
            {
                continue;
            }
            
            Log.ResettingProjection(_logger, identifier.Scope, identifier.Projection, tenant, result.FailureReason);
            tenantsToResetFor.Add(tenant);
        }

        return tenantsToResetFor;
    }

    Task<Try> DropStatesAndResetStreamProcessorsFor(IEnumerable<TenantId> tenants, ProjectionDefinition newDefinition, CancellationToken cancellationToken)
        => _onAllTenants.TryPerformAsync(async (tenant) =>
        {
            if (!tenants.Contains(tenant))
            {
                return Try.Succeeded();
            }
            
            var getDefinition = await _getProjectionDefinitions().TryGet(newDefinition.Projection, newDefinition.Scope, cancellationToken).ConfigureAwait(false);
            if (!getDefinition.Success)
            {
                return new CouldNotResetProjectionStates(newDefinition, tenant);
            }
            var definition = getDefinition.Result;

            try
            {
                await _getStreamProcessorStates().Persist(
                    new StreamProcessorId(definition.Scope, definition.Projection.Value, StreamId.EventLog),
                    StreamProcessorState.New,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return new CouldNotResetProjectionStates(newDefinition, tenant);
            }

            if (!await _getProjectionPersister().TryDrop(definition, cancellationToken).ConfigureAwait(false))
            {
                return new CouldNotResetProjectionStates(newDefinition, tenant);
            }

            return Try.Succeeded();
        });

    Task<Try> PersistDefinitionForAllTenants(ProjectionDefinition definition, CancellationToken cancellationToken)
        => _onAllTenants.TryPerformAsync(async (tenant) =>
        {
            Log.PersistingProjectionDefinition(_logger, definition.Scope, definition.Projection, tenant);
            if (!await _getProjectionDefinitions().TryPersist(definition, cancellationToken).ConfigureAwait(false))
            {
                return new CouldNotPersistProjectionDefinition(definition, tenant);
            }
            return Try.Succeeded();
        });
}
