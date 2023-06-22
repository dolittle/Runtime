// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalGet;
    readonly Counter _totalFailedGet;
    readonly Counter _totalGetAll;
    readonly Counter _totalFailedGetAll;
    readonly Counter _totalReplaceAttempts;
    readonly Counter _totalCopyStoreReplacements;
    readonly Counter _totalFailedCopyStoreReplacements;
    readonly Counter _totalProjectionStoreReplacements;
    readonly Counter _totalFailedProjectionStoreReplacements;
    readonly Counter _totalRemoveAttempts;
    readonly Counter _totalCopyStoreRemovals;
    readonly Counter _totalFailedCopyStoreRemovals;
    readonly Counter _totalProjectionStoreRemovals;
    readonly Counter _totalFailedProjectionStoreRemovals;
    readonly Counter _totalDropAttempts;
    readonly Counter _totalCopyStoreDrops;
    readonly Counter _totalFailedCopyStoreDrops;
    readonly Counter _totalProjectionStoreDrops;
    readonly Counter _totalFailedProjectionStoreDrops;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalGet = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_get_total",
            "ProjectionStore total number of Get requests");
        
        _totalFailedGet = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_get_failed_total",
            "ProjectionStore total number of Get requests that have failed");
        
        _totalGetAll = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_get_all_total",
            "ProjectionStore total number of GetAll requests");
        
        _totalFailedGetAll = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_get_all_failed_total",
            "ProjectionStore total number of GetAll requests that have failed");
        
        _totalReplaceAttempts = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_replace_total",
            "ProjectionPersister total number of Replace requests");
        
        _totalCopyStoreReplacements = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_replace_copy_total",
            "ProjectionPersister total number of Replace in copy stores that succeeded");
        
        _totalFailedCopyStoreReplacements = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_replace_copy_failed_total",
            "ProjectionPersister total number of Replace in copy stores that failed");
        
        _totalProjectionStoreReplacements = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_replace_total",
            "ProjectionStore total number of Replace requests");
        
        _totalFailedProjectionStoreReplacements = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_replace_failed_total",
            "ProjectionStore total number of Replace requests that have failed");
        
        _totalRemoveAttempts = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_remove_total",
            "ProjectionPersister total number of Remove requests");
        
        _totalCopyStoreRemovals = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_remove_copy_total",
            "ProjectionPersister total number of Remove in copy stores that succeeded");
        
        _totalFailedCopyStoreRemovals = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_remove_copy_failed_total",
            "ProjectionPersister total number of Remove in copy stores that failed");
        
        _totalProjectionStoreRemovals = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_remove_total",
            "ProjectionStore total number of Remove requests");
        
        _totalFailedProjectionStoreRemovals = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_remove_failed_total",
            "ProjectionStore total number of Remove requests that have failed");
        
        _totalDropAttempts = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_drop_total",
            "ProjectionPersister total number of Drop requests");
        
        _totalCopyStoreDrops = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_drop_copy_total",
            "ProjectionPersister total number of Drop in copy stores that succeeded");
        
        _totalFailedCopyStoreDrops = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_persister_drop_copy_failed_total",
            "ProjectionPersister total number of Drop in copy stores that failed");
        
        _totalProjectionStoreDrops = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_drop_total",
            "ProjectionStore total number of Drop requests");
        
        _totalFailedProjectionStoreDrops = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_drop_failed_total",
            "ProjectionStore total number of Drop requests that have failed");
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
