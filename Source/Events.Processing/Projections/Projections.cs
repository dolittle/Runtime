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
using Dolittle.Runtime.DependencyInversion.Lifecycle;
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
[Singleton]
public class Projections : IProjections
{
    readonly ConcurrentDictionary<ProjectionIdentifier, ProjectionProcessor> _projections = new();
    
    readonly IStreamProcessors _streamProcessors;
    readonly ICompareProjectionDefinitionsForAllTenants _projectionDefinitionComparer;
    readonly Func<IProjectionPersister> _getProjectionPersister;
    readonly Func<IProjectionStore> _getProjectionStore;
    readonly Func<IProjectionDefinitions> _getProjectionDefinitions;
    readonly Func<IStreamProcessorStateRepository> _getStreamProcessorStates;
    readonly IProjectionKeys _projectionKeys;
    readonly IPerformActionOnAllTenants _onAllTenants;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Projections"/> class.
    /// </summary>
    /// <param name="streamProcessors">The <see cref="IStreamProcessors"/> to use for registering the Projections stream processors.</param>
    /// <param name="projectionDefinitionComparer">The <see cref="ICompareProjectionDefinitionsForAllTenants"/> to use to decide if Projections need to be replayed when registered.</param>
    /// <param name="getProjectionPersister">A <see cref="Func{T}"/> to resolve the <see cref="IProjectionPersister"/> to use to persist Projection read models.</param>
    /// <param name="getProjectionStore">A <see cref="Func{T}"/> to resolve the <see cref="IProjectionStore"/> to use to get the current state while processing events.</param>
    /// <param name="getProjectionDefinitions">A <see cref="Func{T}"/> to resolve the <see cref="IProjectionDefinitions"/> to use to persist new definitions.</param>
    /// <param name="getStreamProcessorStates">A <see cref="Func{T}"/> to resolve the <see cref="IStreamProcessorStateRepository"/> to use to reset the stream processor if a Projection definition has been changed.</param>
    /// <param name="projectionKeys">The <see cref="IProjectionKeys"/> to use to resolve the <see cref="ProjectionKey"/> for events.</param>
    /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants"/> to use to perform actions in the execution context of tenants.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use to create loggers.</param>
    public Projections(
        IStreamProcessors streamProcessors,
        ICompareProjectionDefinitionsForAllTenants projectionDefinitionComparer,
        Func<IProjectionPersister> getProjectionPersister,
        Func<IProjectionStore> getProjectionStore,
        Func<IProjectionDefinitions> getProjectionDefinitions,
        Func<IStreamProcessorStateRepository> getStreamProcessorStates,
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
    public IEnumerable<ProjectionInfo> All => _projections.Values.Select(_ => _.Info);
    
    /// <inheritdoc />
    public Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(ScopeId scopeId, ProjectionId projectionId) 
        => _projections.TryGetValue(new ProjectionIdentifier(scopeId, projectionId), out var processor)
            ? processor.GetCurrentStates()
            : new ProjectionNotRegistered(scopeId, projectionId);

    /// <inheritdoc />
    public async Task<Try<ProjectionProcessor>> Register(IProjection projection, CancellationToken cancellationToken)
    {
        var identifier = new ProjectionIdentifier(projection.Definition.Scope, projection.Definition.Projection);
        Log.RegisteringProjection(_logger, identifier.Scope, identifier.Projection);
        
        if (_projections.ContainsKey(identifier))
        {
            Log.ProjectionAlreadyRegistered(_logger, identifier.Scope, identifier.Projection);
            return new ProjectionAlreadyRegistered(identifier.Scope, identifier.Projection);
        }
        
        Log.RegisteringStreamProcessorForProjection(_logger, identifier.Scope, identifier.Projection);
        
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
            () => _projections.TryRemove(identifier, out _),
            _loggerFactory.CreateLogger<ProjectionProcessor>(),
            cancellationToken);
        
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
    public Task<Try> ReplayEventsForTenant(ScopeId scopeId, ProjectionId projectionId, TenantId tenantId)
    {
        if (!_projections.TryGetValue(new ProjectionIdentifier(scopeId, projectionId), out var processor))
        {
            return Task.FromResult(Try.Failed(new ProjectionNotRegistered(scopeId, projectionId)));
        }

        return processor.ReplayEventsForTenant(tenantId, DropStateAndResetStreamProcessorForTenantCallback(processor));
    }

    /// <inheritdoc />
    public async Task<Try> ReplayEventsForAllTenants(ScopeId scopeId, ProjectionId projectionId)
    {
        if (!_projections.TryGetValue(new ProjectionIdentifier(scopeId, projectionId), out var processor))
        {
            return new ProjectionNotRegistered(scopeId, projectionId);
        }

        var results = await processor.ReplayEventsForAllTenants(DropStateAndResetStreamProcessorForTenantCallback(processor)).ConfigureAwait(false);
        foreach (var (_, result) in results)
        {
            if (!result.Success)
            {
                return result;
            }
        }
        
        return Try.Succeeded();
    }

    async Task<Try> ResetProjectionIfDefinitionHasChanged(ProjectionIdentifier identifier, ProjectionDefinition definition, CancellationToken cancellationToken)
    {
        var tenantsToResetFor = await GetTenantsWhereDefinitionHasChanged(identifier, definition, cancellationToken).ConfigureAwait(false);

        var dropping = await DropStatesAndResetStreamProcessorsFor(tenantsToResetFor, definition, cancellationToken).ConfigureAwait(false);
        if (!dropping.Success)
        {
            return dropping.Exception;
        }

        var persistence = await PersistDefinitionForAllTenants(definition, cancellationToken).ConfigureAwait(false);
        if (!persistence.Success)
        {
            return persistence.Exception;
        }
        
        return Try.Succeeded();
    }

    async Task<IEnumerable<TenantId>> GetTenantsWhereDefinitionHasChanged(ProjectionIdentifier identifier, ProjectionDefinition definition, CancellationToken cancellationToken)
    {
        var tenantsToResetFor = new List<TenantId>();
        
        Log.ComparingProjectionDefiniton(_logger, identifier.Scope, identifier.Projection);
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
            
            Log.ResettingStreamProcessorForTenant(_logger, newDefinition.Scope, newDefinition.Projection, tenant);
            
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
            
            Log.DroppingStatesForTenant(_logger, newDefinition.Scope, newDefinition.Projection, tenant);
            
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

    Func<TenantId, CancellationToken, Task<Try>> DropStateAndResetStreamProcessorForTenantCallback(ProjectionProcessor processor)
        => (tenant, cancellationToken) => DropStatesAndResetStreamProcessorsFor(new []{ tenant }, processor.Definition, cancellationToken);
}
