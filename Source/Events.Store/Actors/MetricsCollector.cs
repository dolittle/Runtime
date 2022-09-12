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
    readonly Counter _totalGet;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalGet = metricFactory.CreateCounter(
            "dolittle_shared_runtime_projections_store_get_total",
            "ProjectionStore total number of Get requests");
        
    }

    /// <inheritdoc />
    public void IncrementTotalGet() 
        => _totalGet.Inc();

    public void IncrementTotalCommitsReceived()
        => throw new System.NotImplementedException();

    public void IncrementTotalCommitsForAggregateReceived()
        => throw new System.NotImplementedException();

    public void IncrementTotalAggregateRootVersionCacheInconsistencies()
        => throw new System.NotImplementedException();

    public void IncrementTotalBatchesSuccessfullyPersisted()
        => throw new System.NotImplementedException();

    public void IncrementTotalBatchedEventsSuccessfullyPersisted(int allEventsCount)
        => throw new System.NotImplementedException();

    public void IncrementTotalBatchesSent()
        => throw new System.NotImplementedException();

    public void IncrementTotalAggregateRootVersionCacheInconsistenciesResolved()
        => throw new System.NotImplementedException();

    public void IncrementTotalBatchesFailedPersisting()
        => throw new System.NotImplementedException();
}
