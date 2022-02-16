// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Singleton]
public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
{
    Counter _totalGet;
    Counter _totalFailedGet;
    Counter _totalGetAll;
    Counter _totalFailedGetAll;
    Counter _totalReplaceAttempts;
    Counter _totalCopyStoreReplacements;
    Counter _totalFailedCopyStoreReplacements;
    Counter _totalProjectionStoreReplacements;
    Counter _totalFailedProjectionStoreReplacements;
    Counter _totalRemoveAttempts;
    Counter _totalCopyStoreRemovals;
    Counter _totalFailedCopyStoreRemovals;
    Counter _totalProjectionStoreRemovals;
    Counter _totalFailedProjectionStoreRemovals;
    Counter _totalDropAttempts;
    Counter _totalCopyStoreDrops;
    Counter _totalFailedCopyStoreDrops;
    Counter _totalProjectionStoreDrops;
    Counter _totalFailedProjectionStoreDrops;
    
    /// <inheritdoc />
    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        _totalGet = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_get_total",
            "ProjectionStore total number of Get requests");
        
        _totalFailedGet = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_get_failed_total",
            "ProjectionStore total number of Get requests that have failed");
        
        _totalGetAll = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_get_all_total",
            "ProjectionStore total number of GetAll requests");
        
        _totalFailedGetAll = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_get_all_failed_total",
            "ProjectionStore total number of GetAll requests that have failed");
        
        _totalReplaceAttempts = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_replace_total",
            "ProjectionPersister total number of Replace requests");
        
        _totalCopyStoreReplacements = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_replace_copy_total",
            "ProjectionPersister total number of Replace in copy stores that succeeded");
        
        _totalFailedCopyStoreReplacements = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_replace_copy_failed_total",
            "ProjectionPersister total number of Replace in copy stores that failed");
        
        _totalProjectionStoreReplacements = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_replace_total",
            "ProjectionStore total number of Replace requests");
        
        _totalFailedProjectionStoreReplacements = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_replace_failed_total",
            "ProjectionStore total number of Replace requests that have failed");
        
        _totalRemoveAttempts = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_remove_total",
            "ProjectionPersister total number of Remove requests");
        
        _totalCopyStoreRemovals = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_remove_copy_total",
            "ProjectionPersister total number of Remove in copy stores that succeeded");
        
        _totalFailedCopyStoreRemovals = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_remove_copy_failed_total",
            "ProjectionPersister total number of Remove in copy stores that failed");
        
        _totalProjectionStoreRemovals = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_remove_total",
            "ProjectionStore total number of Remove requests");
        
        _totalFailedProjectionStoreRemovals = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_remove_failed_total",
            "ProjectionStore total number of Remove requests that have failed");
        
        _totalDropAttempts = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_drop_total",
            "ProjectionPersister total number of Drop requests");
        
        _totalCopyStoreDrops = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_drop_copy_total",
            "ProjectionPersister total number of Drop in copy stores that succeeded");
        
        _totalFailedCopyStoreDrops = metricFactory.Counter(
            "dolittle_shared_runtime_projections_persister_drop_copy_failed_total",
            "ProjectionPersister total number of Drop in copy stores that failed");
        
        _totalProjectionStoreDrops = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_drop_total",
            "ProjectionStore total number of Drop requests");
        
        _totalFailedProjectionStoreDrops = metricFactory.Counter(
            "dolittle_shared_runtime_projections_store_drop_failed_total",
            "ProjectionStore total number of Drop requests that have failed");
        
        return new Collector[]
        {
            _totalGet,
            _totalFailedGet,
            _totalGetAll,
            _totalFailedGetAll,
            _totalReplaceAttempts,
            _totalCopyStoreReplacements,
            _totalFailedCopyStoreReplacements,
            _totalProjectionStoreReplacements,
            _totalFailedProjectionStoreReplacements,
            _totalRemoveAttempts,
            _totalCopyStoreRemovals,
            _totalFailedCopyStoreRemovals,
            _totalProjectionStoreRemovals,
            _totalFailedProjectionStoreRemovals,
            _totalDropAttempts,
            _totalCopyStoreDrops,
            _totalFailedCopyStoreDrops,
            _totalProjectionStoreDrops,
            _totalFailedProjectionStoreDrops,
        };
    }

    /// <inheritdoc />
    public void IncrementTotalGet() 
        => _totalGet.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedGet()
        => _totalFailedGet.Inc();

    /// <inheritdoc />
    public void IncrementTotalGetAll()
        => _totalGetAll.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedGetAll()
        => _totalFailedGetAll.Inc();

    /// <inheritdoc />
    public void IncrementTotalReplaceAttempts()
        => _totalReplaceAttempts.Inc();

    /// <inheritdoc />
    public void IncrementTotalCopyStoreReplacements()
        => _totalCopyStoreReplacements.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedCopyStoreReplacements()
        => _totalFailedCopyStoreReplacements.Inc();

    /// <inheritdoc />
    public void IncrementTotalProjectionStoreReplacements()
        => _totalProjectionStoreReplacements.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedProjectionStoreReplacements()
        => _totalFailedProjectionStoreReplacements.Inc();

    /// <inheritdoc />
    public void IncrementTotalRemoveAttempts()
        => _totalRemoveAttempts.Inc();

    /// <inheritdoc />
    public void IncrementTotalCopyStoreRemovals()
        => _totalCopyStoreRemovals.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedCopyStoreRemovals()
        => _totalFailedCopyStoreRemovals.Inc();

    /// <inheritdoc />
    public void IncrementTotalProjectionStoreRemovals()
        => _totalProjectionStoreRemovals.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedProjectionStoreRemovals()
        => _totalFailedProjectionStoreRemovals.Inc();

    /// <inheritdoc />
    public void IncrementTotalDropAttempts()
        => _totalDropAttempts.Inc();

    /// <inheritdoc />
    public void IncrementTotalCopyStoreDrops()
        => _totalCopyStoreDrops.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedCopyStoreDrops()
        => _totalFailedCopyStoreDrops.Inc();

    /// <inheritdoc />
    public void IncrementTotalProjectionStoreDrops()
        => _totalProjectionStoreDrops.Inc();

    /// <inheritdoc />
    public void IncrementTotalFailedProjectionStoreDrops()
        => _totalFailedProjectionStoreDrops.Inc();
}
