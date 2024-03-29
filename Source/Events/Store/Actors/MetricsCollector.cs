// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly IEventTypes _eventTypes;
    readonly IAggregateRoots _aggregateRoots;
    readonly Counter _totalCommitsReceived;
    readonly Counter _totalCommitsForAggregateReceived;
    readonly Counter _totalAggregateRootVersionCacheInconsistencies;
    readonly Counter _totalBatchesSuccessfullyPersisted;
    readonly Counter _totalBatchedEventsSuccessfullyPersisted;
    readonly Counter _totalBatchesSent;
    readonly Counter _totalAggregateRootVersionCacheInconsistenciesResolved;
    readonly Counter _totalBatchesFailedPersisting;
    readonly Counter _totalCommittedEvents;
    readonly Counter _totalCommittedAggregateEvents;
    readonly Counter _totalAggregateConcurrencyConflicts;
    readonly Counter _streamedSubscriptionEventsTotal;
    readonly Counter _catchupSubscriptionEventsTotal;

    public MetricsCollector(IMetricFactory metricFactory, IEventTypes eventTypes, IAggregateRoots aggregateRoots)
    {
        _eventTypes = eventTypes;
        _aggregateRoots = aggregateRoots;
        _totalCommitsReceived = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_commits_received_total",
            "EventStore total number of non-aggregate commits received");
        
        _totalCommitsForAggregateReceived = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_commits_for_aggregate_received_total",
            "EventStore total number of commits for aggregate received");
        
        _totalAggregateRootVersionCacheInconsistencies = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_aggregate_root_version_cache_inconsistencies_total",
            "EventStore total number of aggregate root version cache inconsistencies occurred");
        
        _totalBatchesSuccessfullyPersisted = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_batches_successfully_persisted_total",
            "EventStore total number of batches that has been successfully persisted");
        
        _totalBatchedEventsSuccessfullyPersisted = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_batched_events_successfully_persisted_total",
            "EventStore total number of batched events that has been successfully persisted");
         
        _totalBatchesSent = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_batches_sent_total",
            "EventStore total number of batches that has been sent to the event store");
        
        _totalAggregateRootVersionCacheInconsistenciesResolved = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_aggregate_root_version_cache_inconsistencies_resolved_total",
            "EventStore total number of aggregate root version cache inconsistencies that has been resolved");

        _totalBatchesFailedPersisting = metricFactory.CreateCounter(
            "dolittle_system_runtime_events_store_batches_failed_persisting_total",
            "EventStore total number of batches that failed to be persisted");

        _totalCommittedEvents = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_committed_events_total",
            "EventStore total number of committed events by type",
            new[] {"tenantId", "eventTypeId", "eventTypeAlias"});
        
        _totalCommittedAggregateEvents = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_committed_aggregate_events_total",
            "EventStore total number of committed events by type",
            new[] {"tenantId", "eventTypeId", "eventTypeAlias", "aggregateRootId", "aggregateRootAlias"});
        
        _totalAggregateConcurrencyConflicts = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_aggregate_concurrency_conflicts_total",
            "EventStore total number of aggregate concurrency conflicts by aggregate root",
            new[] {"tenantId", "aggregateRootId", "aggregateRootAlias"});
        
        _streamedSubscriptionEventsTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_streamed_events_total",
            "Total number of directly streamed events",
            new[] { "subscriptionName" });

        _catchupSubscriptionEventsTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_catchup_events_total",
            "Total number of catchup-events (events that are not streamed directly, but read from DB)",
            new[] { "subscriptionName" });
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
    public void IncrementTotalBatchesSuccessfullyPersisted(Commit commit)
    {
        _totalBatchesSuccessfullyPersisted.Inc();
        foreach (var committedEvents in commit.Events)
        {
            foreach (var committedEvent in committedEvents)
            {
                _totalCommittedEvents
                    .WithLabels(
                        committedEvent.ExecutionContext.Tenant.ToString(),
                        committedEvent.Type.Id.ToString(),
                        _eventTypes.GetEventTypeAliasOrEmptyString(committedEvent.Type)
                    ).Inc();
                _totalBatchedEventsSuccessfullyPersisted.Inc();
            }
        }
        foreach (var commitAggregateEvents in commit.AggregateEvents)
        {
            foreach (var committedEvent in commitAggregateEvents)
            {
                _totalCommittedEvents
                    .WithLabels(
                        committedEvent.ExecutionContext.Tenant.ToString(),
                        committedEvent.Type.Id.ToString(),
                        _eventTypes.GetEventTypeAliasOrEmptyString(committedEvent.Type)
                    ).Inc();
                _totalCommittedAggregateEvents
                    .WithLabels(
                        committedEvent.ExecutionContext.Tenant.ToString(),
                        committedEvent.Type.Id.ToString(),
                        _eventTypes.GetEventTypeAliasOrEmptyString(committedEvent.Type),
                        committedEvent.AggregateRoot.Id.ToString(),
                        GetAggregateRootAliasOrEmptyString(committedEvent.AggregateRoot.Id)
                    ).Inc();
                _totalBatchedEventsSuccessfullyPersisted.Inc();
            }
        }
    }

    /// <inheritdoc />
    public void IncrementTotalBatchesSent()
        => _totalBatchesSent.Inc();

    /// <inheritdoc />
    public void IncrementTotalAggregateRootVersionCacheInconsistenciesResolved()
        => _totalAggregateRootVersionCacheInconsistenciesResolved.Inc();

    /// <inheritdoc />
    public void IncrementTotalBatchesFailedPersisting()
        => _totalBatchesFailedPersisting.Inc();

    public void IncrementTotalAggregateRootConcurrencyConflicts(TenantId tenant, ArtifactId aggregateRoot)
        => _totalAggregateConcurrencyConflicts
            .WithLabels(
                tenant.ToString(),
                aggregateRoot.ToString(),
                GetAggregateRootAliasOrEmptyString(aggregateRoot)
            ).Inc();

    string GetAggregateRootAliasOrEmptyString(ArtifactId aggregateRoot)
    {
        if (!_aggregateRoots.TryGetFor(aggregateRoot, out var aggregateRootInfo))
        {
            return string.Empty;
        }

        return aggregateRootInfo!.Alias == AggregateRootAlias.NotSet
            ? string.Empty
            : aggregateRootInfo.Alias.Value;
    }
    
    
    public void IncrementStreamedSubscriptionEvents(string subscriptionName, int incBy)
        => _streamedSubscriptionEventsTotal.WithLabels(subscriptionName).Inc(incBy);

    public void IncrementCatchupSubscriptionEvents(string subscriptionName, int incBy)
        => _catchupSubscriptionEventsTotal.WithLabels(subscriptionName).Inc(incBy);
}
