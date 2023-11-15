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
public class MetricsCollector(IMetricFactory metricFactory) : IMetricsCollector
{
    readonly Counter _totalGet = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_get_total",
        "ProjectionStore total number of Get requests");
    readonly Counter _totalFailedGet = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_get_failed_total",
        "ProjectionStore total number of Get requests that have failed");
    readonly Counter _totalGetAll = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_get_all_total",
        "ProjectionStore total number of GetAll requests");
    readonly Counter _totalFailedGetAll = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_get_all_failed_total",
        "ProjectionStore total number of GetAll requests that have failed");
    readonly Counter _totalReplaceAttempts = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_replace_total",
        "ProjectionPersister total number of Replace requests");
    readonly Counter _totalCopyStoreReplacements = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_replace_copy_total",
        "ProjectionPersister total number of Replace in copy stores that succeeded");
    readonly Counter _totalFailedCopyStoreReplacements = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_replace_copy_failed_total",
        "ProjectionPersister total number of Replace in copy stores that failed");
    readonly Counter _totalProjectionStoreReplacements = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_replace_total",
        "ProjectionStore total number of Replace requests");
    readonly Counter _totalFailedProjectionStoreReplacements = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_replace_failed_total",
        "ProjectionStore total number of Replace requests that have failed");
    readonly Counter _totalRemoveAttempts = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_remove_total",
        "ProjectionPersister total number of Remove requests");
    readonly Counter _totalCopyStoreRemovals = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_remove_copy_total",
        "ProjectionPersister total number of Remove in copy stores that succeeded");
    readonly Counter _totalFailedCopyStoreRemovals = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_remove_copy_failed_total",
        "ProjectionPersister total number of Remove in copy stores that failed");
    readonly Counter _totalProjectionStoreRemovals = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_remove_total",
        "ProjectionStore total number of Remove requests");
    readonly Counter _totalFailedProjectionStoreRemovals = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_remove_failed_total",
        "ProjectionStore total number of Remove requests that have failed");
    readonly Counter _totalDropAttempts = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_drop_total",
        "ProjectionPersister total number of Drop requests");
    readonly Counter _totalCopyStoreDrops = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_drop_copy_total",
        "ProjectionPersister total number of Drop in copy stores that succeeded");
    readonly Counter _totalFailedCopyStoreDrops = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_persister_drop_copy_failed_total",
        "ProjectionPersister total number of Drop in copy stores that failed");
    readonly Counter _totalProjectionStoreDrops = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_drop_total",
        "ProjectionStore total number of Drop requests");
    readonly Counter _totalFailedProjectionStoreDrops = metricFactory.CreateCounter(
        "dolittle_shared_runtime_projections_store_drop_failed_total",
        "ProjectionStore total number of Drop requests that have failed");

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
