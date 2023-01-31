// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

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
    readonly Func<TenantId, ProjectionDefinition, IProjection, EventProcessor> _createEventProcessor;
    readonly Func<IProjection, StreamProcessor, Action, CancellationToken, ProjectionProcessor> _createProjectionProcessor;
    readonly IPerformActionsForAllTenants _forAllTenants;
    readonly ILogger _logger;
    readonly EventProcessorKind _kind = "Projection";

    /// <summary>
    /// Initializes a new instance of the <see cref="Projections"/> class.
    /// </summary>
    /// <param name="streamProcessors">The <see cref="IStreamProcessors"/> to use for registering the Projections stream processors.</param>
    /// <param name="projectionDefinitionComparer">The <see cref="ICompareProjectionDefinitionsForAllTenants"/> to use to decide if Projections need to be replayed when registered.</param>
    /// <param name="createEventProcessor">The factor to use to create projection event processors.</param>
    /// <param name="createProjectionProcessor">The factory to use to create projection processors.</param>
    /// <param name="forAllTenants">The <see cref="IPerformActionsForAllTenants"/> to use to perform actions in the execution context of tenants.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public Projections(
        IStreamProcessors streamProcessors,
        ICompareProjectionDefinitionsForAllTenants projectionDefinitionComparer,
        Func<TenantId, ProjectionDefinition, IProjection, EventProcessor> createEventProcessor, // TODO: This should maybe return the interface?
        Func<IProjection, StreamProcessor, Action, CancellationToken, ProjectionProcessor> createProjectionProcessor,
        IPerformActionsForAllTenants forAllTenants,
        ILogger logger)
    {
        _streamProcessors = streamProcessors;
        _projectionDefinitionComparer = projectionDefinitionComparer;
        _createEventProcessor = createEventProcessor;
        _createProjectionProcessor = createProjectionProcessor;
        _forAllTenants = forAllTenants;
        _logger = logger;
    }

    /// <inheritdoc />
    public IEnumerable<ProjectionInfo> All => _projections.Values.Select(_ => _.Info);
    
    /// <inheritdoc />
    public Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(ScopeId scopeId, ProjectionId projectionId) 
        => _projections.TryGetValue(new ProjectionIdentifier(scopeId, projectionId), out var processor)
            ? processor.GetCurrentStates()
            : new ProjectionNotRegistered(scopeId, projectionId);

    /// <inheritdoc />
    public async Task<Try<ProjectionProcessor>> Register(IProjection projection, ExecutionContext executionContext, CancellationToken cancellationToken)
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
            _kind,
            new EventLogStreamDefinition(),
            (tenant) => _createEventProcessor(tenant, projection.Definition, projection),
            executionContext,
            cancellationToken);

        if (!registration.Success)
        {
            Log.FailedToRegisterProjectionStreamProcessor(_logger, identifier.Scope, identifier.Projection, registration.Exception);
            return registration.Exception;
        }
        
        var processor = _createProjectionProcessor(
            projection,
            registration.Result,
            () => _projections.TryRemove(identifier, out _),
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
        
        return Try.Succeeded;
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
        
        return Try.Succeeded;
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
        => _forAllTenants.TryPerformAsync(async (tenant, services) =>
        {
            if (!tenants.Contains(tenant))
            {
                return Try.Succeeded;
            }
            
            Log.ResettingStreamProcessorForTenant(_logger, newDefinition.Scope, newDefinition.Projection, tenant);
            
            var projectionDefinitions = services.GetRequiredService<IProjectionDefinitions>();
            var getDefinition = await projectionDefinitions.TryGet(newDefinition.Projection, newDefinition.Scope, cancellationToken).ConfigureAwait(false);
            if (!getDefinition.Success)
            {
                return new CouldNotResetProjectionStates(newDefinition, tenant);
            }
            var definition = getDefinition.Result;

            try
            {
                var streamProcessorStates = services.GetRequiredService<IStreamProcessorStates>();
                await streamProcessorStates.Persist(
                    new StreamProcessorId(definition.Scope, definition.Projection.Value, StreamId.EventLog),
                    StreamProcessorState.New,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return new CouldNotResetProjectionStates(newDefinition, tenant);
            }
            
            Log.DroppingStatesForTenant(_logger, newDefinition.Scope, newDefinition.Projection, tenant);
            
            var projectionPersister = services.GetRequiredService<IProjectionPersister>();
            if (!await projectionPersister.TryDrop(definition, cancellationToken).ConfigureAwait(false))
            {
                return new CouldNotResetProjectionStates(newDefinition, tenant);
            }

            return Try.Succeeded;
        });

    Task<Try> PersistDefinitionForAllTenants(ProjectionDefinition definition, CancellationToken cancellationToken)
        => _forAllTenants.TryPerformAsync(async (tenant, services) =>
        {
            Log.PersistingProjectionDefinition(_logger, definition.Scope, definition.Projection, tenant);
            var projectionDefinitions = services.GetRequiredService<IProjectionDefinitions>();
            if (!await projectionDefinitions.TryPersist(definition, cancellationToken).ConfigureAwait(false))
            {
                return new CouldNotPersistProjectionDefinition(definition, tenant);
            }
            return Try.Succeeded;
        });

    Func<TenantId, CancellationToken, Task<Try>> DropStateAndResetStreamProcessorForTenantCallback(ProjectionProcessor processor)
        => (tenant, cancellationToken) => DropStatesAndResetStreamProcessorsFor(new []{ tenant }, processor.Definition, cancellationToken);
}
