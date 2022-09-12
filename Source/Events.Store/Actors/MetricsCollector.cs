// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalCommitsReceived;
    readonly Counter _totalCommitsForAggregateReceived;
    readonly Counter _totalAggregateRootVersionCacheInconsistencies;
    readonly Counter _totalBatchesSuccessfullyPersisted;
    readonly Counter _totalBatchedEventsSuccessfullyPersisted;
    readonly Counter _totalBatchesSent;
    readonly Counter _totalAggregateRootVersionCacheInconsistenciesResolved;
    readonly Counter _totalBatchesFailedPersisting;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalCommitsReceived = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_commits_received_total",
            "EventStore total number of non-aggregate commits received");
        
        _totalCommitsForAggregateReceived = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_commits_for_aggregate_received_total",
            "EventStore total number of commits for aggregate received");
        
        _totalAggregateRootVersionCacheInconsistencies = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_aggregate_root_version_cache_inconsistencies_total",
            "EventStore total number of aggregate root version cache inconsistencies occurred");
        
        _totalBatchesSuccessfullyPersisted = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_batches_successfully_persisted_total",
            "EventStore total number of batches that has been successfully persisted");
        
        _totalBatchedEventsSuccessfullyPersisted = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_batched_events_successfully_persisted_total",
            "EventStore total number of batched events that has been successfully persisted");
         
        _totalBatchesSent = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_batches_sent_total",
            "EventStore total number of batches that has been sent to the event store");
        
        _totalAggregateRootVersionCacheInconsistenciesResolved = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_aggregate_root_version_cache_inconsistencies_resolved_total",
            "EventStore total number of aggregate root version cache inconsistencies that has been resolved");

        _totalBatchesFailedPersisting = metricFactory.CreateCounter(
            "dolittle_shared_runtime_events_store_batches_failed_persisting_total",
            "EventStore total number of batches that failed to be persisted");
    }

    /// <inheritdoc />
    public void IncrementTotalCommitsReceived()
        => _totalCommitsReceived.Inc();

    /// <inheritdoc />
    public void IncrementTotalCommitsForAggregateReceived()
        => _totalCommitsForAggregateReceived.Inc();

    /// <inheritdoc />
    public void IncrementTotalAggregateRootVersionCacheInconsistencies()
        => _totalAggregateRootVersionCacheInconsistencies.Inc();

    /// <inheritdoc />
    public void IncrementTotalBatchesSuccessfullyPersisted()
        => _totalBatchesSuccessfullyPersisted.Inc();

    /// <inheritdoc />
    public void IncrementTotalBatchedEventsSuccessfullyPersisted(int allEventsCount)
        => _totalBatchedEventsSuccessfullyPersisted.Inc(allEventsCount);

    /// <inheritdoc />
    public void IncrementTotalBatchesSent()
        => _totalBatchesSent.Inc();

    /// <inheritdoc />
    public void IncrementTotalAggregateRootVersionCacheInconsistenciesResolved()
        => _totalAggregateRootVersionCacheInconsistenciesResolved.Inc();

    /// <inheritdoc />
    public void IncrementTotalBatchesFailedPersisting()
        => _totalBatchesFailedPersisting.Inc();
}
