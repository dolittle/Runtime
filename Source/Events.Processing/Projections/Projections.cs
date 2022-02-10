// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    readonly IProjectionKeys _projectionKeys;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;

    public Projections(
        IStreamProcessors streamProcessors,
        ICompareProjectionDefinitionsForAllTenants projectionDefinitionComparer,
        FactoryFor<IProjectionPersister> getProjectionPersister,
        FactoryFor<IProjectionStore> getProjectionStore,
        IProjectionKeys projectionKeys,
        ILoggerFactory loggerFactory)
    {
        _streamProcessors = streamProcessors;
        _projectionDefinitionComparer = projectionDefinitionComparer;
        _getProjectionPersister = getProjectionPersister;
        _getProjectionStore = getProjectionStore;
        _projectionKeys = projectionKeys;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<Projections>();
    }
    
    /// <inheritdoc />
    public IEnumerable<ProjectionDefinition> All { get; }
    
    /// <inheritdoc />
    public Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(ScopeId scopeId, ProjectionId projectionId) => throw new System.NotImplementedException();

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

        var tenantComparisonResults = await _projectionDefinitionComparer.DiffersFromPersisted(projection.Definition, cancellationToken).ConfigureAwait(false);
        var tenantsToResetFor = tenantComparisonResults
            .Where(_ =>
            {
                var (tenantId, result) = _;
                if (result.Succeeded)
                {
                    return false;
                }
                
                Log.ResettingProjection(_logger, identifier.Scope, identifier.Projection, tenantId, result.FailureReason);
                return true;
            })
            .Select(_ => _.Key);
        
        var dropping = await DropStatesAndResetStreamProcessorsFor(tenantsToResetFor, cancellationToken);
        if (!dropping.Success)
        {
            processor.Dispose();
            return dropping.Exception;
        }

        var persistence = await PersistDefinitionForAllTenants(projection.Definition, cancellationToken);
        if (!persistence.Success)
        {
            processor.Dispose();
            return persistence.Exception;
        }

        return processor;
    }

    /// <inheritdoc />
    public Task<Try> ReplayEventsForTenant(ScopeId scopeId, ProjectionId projectionId, TenantId tenantId) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public Task<Try> ReplayEventsForAllTenants(ScopeId scopeId, ProjectionId projectionId) => throw new System.NotImplementedException();

    Task<Try> DropStatesAndResetStreamProcessorsFor(IEnumerable<TenantId> tenants, CancellationToken cancellationToken)
    {
        
    }

    Task<Try> PersistDefinitionForAllTenants(ProjectionDefinition definition, CancellationToken cancellationToken)
    {
        
    }
}
