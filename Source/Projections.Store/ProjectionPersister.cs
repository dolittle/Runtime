// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Projections.Store.Copies;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionPersister"/>.
/// </summary>
[SingletonPerTenant]
public class ProjectionPersister : IProjectionPersister
{
    readonly IProjectionStore _projectionStore;
    readonly IEnumerable<IProjectionCopyStore> _copyStores;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionPersister"/> class.
    /// </summary>
    /// <param name="projectionStore">The projection store to persist to.</param>
    /// <param name="copyStores">The copy stores to persist to.</param>
    /// <param name="metrics">The metrics collector to use.</param>
    /// <param name="logger">The logger to use.</param>
    public ProjectionPersister(
        IProjectionStore projectionStore,
        IEnumerable<IProjectionCopyStore> copyStores,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _projectionStore = projectionStore;
        _copyStores = copyStores;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task<bool> TryReplace(ProjectionDefinition projection, ProjectionKey key, ProjectionState state, CancellationToken token)
    {
        Log.ReplacingProjectionState(_logger, projection.Projection, projection.Scope, key);
        _metrics.IncrementTotalReplaceAttempts();
        foreach (var store in ApplicableCopyStores(projection))
        {
            if (!await store.TryReplace(projection, key, state, token).ConfigureAwait(false))
            {
                Log.FailedToReplaceProjectionStateInCopyStore(_logger, projection.Projection, projection.Scope, key);
                _metrics.IncrementTotalFailedCopyStoreReplacements();
                return false;
            }
            _metrics.IncrementTotalCopyStoreReplacements();
        }
        
        return await _projectionStore.TryReplace(projection.Projection, projection.Scope, key, state, token);
    }

    public async Task<bool> TryRemove(ProjectionDefinition projection, ProjectionKey key, CancellationToken token)
    {
        Log.RemovingProjectionState(_logger, projection.Projection, projection.Scope, key);
        _metrics.IncrementTotalRemoveAttempts();
        foreach (var store in ApplicableCopyStores(projection))
        {
            if (!await store.TryRemove(projection, key, token).ConfigureAwait(false))
            {
                Log.FailedToRemoveProjectionStateInCopyStore(_logger, projection.Projection, projection.Scope, key);
                _metrics.IncrementTotalFailedCopyStoreRemovals();
                return false;
            }
            _metrics.IncrementTotalCopyStoreRemovals();
        }
        
        return await _projectionStore.TryRemove(projection.Projection, projection.Scope, key, token);
    }

    public async Task<bool> TryDrop(ProjectionDefinition projection, CancellationToken token)
    {
        Log.DroppingProjection(_logger, projection.Projection, projection.Scope);
        _metrics.IncrementTotalDropAttempts();
        foreach (var store in ApplicableCopyStores(projection))
        {
            if (!await store.TryDrop(projection, token).ConfigureAwait(false))
            {
                Log.FailedToDropProjectionInCopyStore(_logger, projection.Projection, projection.Scope);
                _metrics.IncrementTotalFailedCopyStoreDrops();
                return false;
            }
            _metrics.IncrementTotalCopyStoreDrops();
        }
        
        return await _projectionStore.TryDrop(projection.Projection, projection.Scope, token);
    }

    IEnumerable<IProjectionCopyStore> ApplicableCopyStores(ProjectionDefinition projection)
        => _copyStores.Where(_ => _.ShouldPersistFor(projection));
}
