// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Metrics;
using Dolittle.Runtime.Protobuf;
using Prometheus;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly IEventTypes _eventTypes;
    readonly IEventHandlers _eventHandlers;
    readonly IAggregateRoots _aggregateRoots;

    // Prometheus
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

    readonly ConcurrentDictionary<TenantId, Func<long>> _eventLogOffsets = new();

    readonly HashSet<(TenantId, StreamProcessorId)> _streamProcessorEventLogOffsets = new();

    // OpenTelemetry
    readonly Counter<long> _totalCommitsReceivedOtel;
    readonly Counter<long> _totalCommitsForAggregateReceivedOtel;
    readonly Counter<long> _totalAggregateRootVersionCacheInconsistenciesOtel;
    readonly Counter<long> _totalBatchesSuccessfullyPersistedOtel;
    readonly Counter<long> _totalBatchedEventsSuccessfullyPersistedOtel;
    readonly Counter<long> _totalBatchesSentOtel;
    readonly Counter<long> _totalAggregateRootVersionCacheInconsistenciesResolvedOtel;
    readonly Counter<long> _totalBatchesFailedPersistingOtel;
    readonly Counter<long> _totalCommittedEventsOtel;
    readonly Counter<long> _totalCommittedAggregateEventsOtel;
    readonly Counter<long> _totalAggregateConcurrencyConflictsOtel;
    readonly Counter<long> _streamedSubscriptionEventsTotalOtel;
    readonly Counter<long> _catchupSubscriptionEventsTotalOtel;


    public MetricsCollector(IMetricFactory metricFactory, IEventTypes eventTypes, IEventHandlers eventHandlers, IAggregateRoots aggregateRoots)
    {
        _eventTypes = eventTypes;
        _eventHandlers = eventHandlers;
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
            new[] { "tenantId", "eventTypeId", "eventTypeAlias" });

        _totalCommittedAggregateEvents = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_committed_aggregate_events_total",
            "EventStore total number of committed events by type",
            new[] { "tenantId", "eventTypeId", "eventTypeAlias", "aggregateRootId", "aggregateRootAlias" });

        _totalAggregateConcurrencyConflicts = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_aggregate_concurrency_conflicts_total",
            "EventStore total number of aggregate concurrency conflicts by aggregate root",
            new[] { "tenantId", "aggregateRootId", "aggregateRootAlias" });

        _streamedSubscriptionEventsTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_streamed_events_total",
            "Total number of directly streamed events",
            new[] { "subscriptionName" });

        _catchupSubscriptionEventsTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_events_store_catchup_events_total",
            "Total number of catchup-events (events that are not streamed directly, but read from DB)",
            new[] { "subscriptionName" });


        // Otel
        _totalCommitsReceivedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_commits_received_total",
            "count",
            "EventStore total number of non-aggregate commits received");

        _totalCommitsForAggregateReceivedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_commits_for_aggregate_received_total",
            "count",
            "EventStore total number of commits for aggregate received");

        _totalAggregateRootVersionCacheInconsistenciesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_aggregate_root_version_cache_inconsistencies_total",
            "count",
            "EventStore total number of aggregate root version cache inconsistencies occurred");

        _totalBatchesSuccessfullyPersistedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_batches_successfully_persisted_total",
            "count",
            "EventStore total number of batches that has been successfully persisted");

        _totalBatchedEventsSuccessfullyPersistedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_batched_events_successfully_persisted_total",
            "count",
            "EventStore total number of batched events that has been successfully persisted");

        _totalBatchesSentOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_batches_sent_total",
            "count",
            "EventStore total number of batches that has been sent to the event store");

        _totalAggregateRootVersionCacheInconsistenciesResolvedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_aggregate_root_version_cache_inconsistencies_resolved_total",
            "count",
            "EventStore total number of aggregate root version cache inconsistencies that has been resolved");

        _totalBatchesFailedPersistingOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_events_store_batches_failed_persisting_total",
            "count",
            "EventStore total number of batches that failed to be persisted");

        _totalCommittedEventsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_events_store_committed_events_total",
            "count",
            "EventStore total number of committed events by type");


        _totalCommittedAggregateEventsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_events_store_committed_aggregate_events_total",
            "count",
            "EventStore total number of committed events by type");

        _totalAggregateConcurrencyConflictsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_events_store_aggregate_concurrency_conflicts_total",
            "count",
            "EventStore total number of aggregate concurrency conflicts by aggregate root");

        _streamedSubscriptionEventsTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_events_store_streamed_events_total",
            "count",
            "Total number of directly streamed events");

        _catchupSubscriptionEventsTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_events_store_catchup_events_total",
            "count",
            "Total number of catchup-events (events that are not streamed directly, but read from DB)");
    }

    /// <inheritdoc />
    public void IncrementTotalCommitsReceived()
    {
        _totalCommitsReceived.Inc();
        _totalCommitsReceivedOtel.Add(1);
    }

    /// <inheritdoc />
    public void IncrementTotalCommitsForAggregateReceived()
    {
        _totalCommitsForAggregateReceived.Inc();
        _totalCommitsForAggregateReceivedOtel.Add(1);
    }

    /// <inheritdoc />
    public void IncrementTotalAggregateRootVersionCacheInconsistencies()
    {
        _totalAggregateRootVersionCacheInconsistencies.Inc();
        _totalAggregateRootVersionCacheInconsistenciesOtel.Add(1);
    }

    /// <inheritdoc />
    public void IncrementTotalBatchesSuccessfullyPersisted(TenantId tenant, Commit commit)
    {
        _totalBatchesSuccessfullyPersisted.Inc();
        _totalBatchesSuccessfullyPersistedOtel.Add(1);
        var tenantId = tenant.ToString();
        var tenantLabel = new KeyValuePair<string, object?>("tenantId", tenantId);

        foreach (var committedEvents in commit.Events)
        {
            foreach (var committedEvent in committedEvents)
            {
                var eventTypeAliasOrEmptyString = _eventTypes.GetEventTypeAliasOrEmptyString(committedEvent.Type);
                var typeId = committedEvent.Type.Id.ToString();
                var labels = new[]
                {
                    tenantLabel,
                    new KeyValuePair<string, object?>("eventTypeId", typeId),
                    new KeyValuePair<string, object?>("eventTypeAlias", eventTypeAliasOrEmptyString)
                };
                _totalCommittedEvents
                    .WithLabels(
                        tenantId,
                        typeId,
                        eventTypeAliasOrEmptyString
                    ).Inc();
                _totalCommittedEventsOtel.Add(1, labels);
                _totalBatchedEventsSuccessfullyPersisted.Inc();
                _totalBatchedEventsSuccessfullyPersistedOtel.Add(1);
            }
        }

        foreach (var commitAggregateEvents in commit.AggregateEvents)
        {
            foreach (var committedEvent in commitAggregateEvents)
            {
                var eventTypeId = committedEvent.Type.Id.ToString();
                var eventTypeIdLabel = new KeyValuePair<string, object?>("eventTypeId", eventTypeId);
                var eventTypeAlias = _eventTypes.GetEventTypeAliasOrEmptyString(committedEvent.Type);
                var eventTypeAliasLabel = new KeyValuePair<string, object?>("eventTypeAlias", eventTypeAlias);


                _totalCommittedEvents
                    .WithLabels(
                        tenantId,
                        eventTypeId,
                        eventTypeAlias
                    ).Inc();
                _totalCommittedEventsOtel.Add(1, tenantLabel, eventTypeIdLabel, eventTypeAliasLabel);

                var aggregateRootId = committedEvent.AggregateRoot.Id.ToString();
                var aggregateRootIdLabel = new KeyValuePair<string, object?>("aggregateRootId", aggregateRootId);
                var aggregateRootAlias = GetAggregateRootAliasOrEmptyString(committedEvent.AggregateRoot.Id);
                var aggregateRootAliasLabel =
                    new KeyValuePair<string, object?>("aggregateRootAlias", aggregateRootAlias);
                _totalCommittedAggregateEvents
                    .WithLabels(
                        tenantId,
                        eventTypeId,
                        eventTypeAlias,
                        aggregateRootId,
                        aggregateRootAlias
                    ).Inc();
                _totalCommittedAggregateEventsOtel.Add(1,
                    tenantLabel, eventTypeIdLabel, eventTypeAliasLabel,
                    aggregateRootIdLabel, aggregateRootAliasLabel);
                _totalBatchedEventsSuccessfullyPersisted.Inc();
                _totalBatchedEventsSuccessfullyPersistedOtel.Add(1);
            }
        }
    }

    /// <inheritdoc />
    public void IncrementTotalBatchesSent()
    {
        _totalBatchesSent.Inc();
        _totalBatchesSentOtel.Add(1);
    }

    /// <inheritdoc />
    public void IncrementTotalAggregateRootVersionCacheInconsistenciesResolved()
    {
        _totalAggregateRootVersionCacheInconsistenciesResolved.Inc();
        _totalAggregateRootVersionCacheInconsistenciesResolvedOtel.Add(1);
    }

    /// <inheritdoc />
    public void IncrementTotalBatchesFailedPersisting()
    {
        _totalBatchesFailedPersisting.Inc();
        _totalBatchesFailedPersistingOtel.Add(1);
    }

    public void IncrementTotalAggregateRootConcurrencyConflicts(TenantId tenant, ArtifactId aggregateRoot)
    {
        var tenantId = tenant.ToString();
        var aggregateId = aggregateRoot.ToString();
        var aggregateAlias = GetAggregateRootAliasOrEmptyString(aggregateRoot);
        _totalAggregateConcurrencyConflicts
            .WithLabels(
                tenantId,
                aggregateId,
                aggregateAlias
            ).Inc();
        _totalAggregateConcurrencyConflictsOtel.Add(1,
            new KeyValuePair<string, object?>("tenantId", tenantId),
            new KeyValuePair<string, object?>("aggregateRootId", aggregateId),
            new KeyValuePair<string, object?>("aggregateRootAlias", aggregateAlias)
        );
    }

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
    {
        _streamedSubscriptionEventsTotal.WithLabels(subscriptionName).Inc(incBy);
        _streamedSubscriptionEventsTotalOtel.Add(incBy,
            new KeyValuePair<string, object?>("subscriptionName", subscriptionName));
    }

    public void IncrementCatchupSubscriptionEvents(string subscriptionName, int incBy)
    {
        _catchupSubscriptionEventsTotal.WithLabels(subscriptionName).Inc(incBy);
        _catchupSubscriptionEventsTotalOtel.Add(incBy,
            new KeyValuePair<string, object?>("subscriptionName", subscriptionName));
    }

    public void RegisterStreamProcessorOffset(TenantId tenant, StreamProcessorId streamProcessorId, Func<ProcessingPosition> getNextProcessingPosition)
    {
        var key = (tenant, streamProcessorId);
        if (_streamProcessorEventLogOffsets.Contains(key)) return;

        var processorId = streamProcessorId.EventProcessorId.ToGuid().ToString();
        var scopeId = streamProcessorId.ScopeId.ToGuid().ToString();
        var eventHandlerInfo = _eventHandlers.All.FirstOrDefault(it => it.Id.EventHandler.Value.ToString().Equals(processorId) && it.Id.Scope.Value.ToString().Equals(scopeId));
        var alias = eventHandlerInfo?.HasAlias == true ? eventHandlerInfo.Alias.Value : string.Empty;

        
        var labels = new[]
        {
            new KeyValuePair<string, object?>("tenantId", tenant.ToString()),
            new KeyValuePair<string, object?>("scopeId", scopeId),
            new KeyValuePair<string, object?>("eventProcessorId", processorId),
            new KeyValuePair<string, object?>("alias", alias),
        };

        RuntimeMetrics.Meter.CreateObservableCounter(
            $"dolittle_customer_runtime_stream_processors_offset",
            GetEventLogPositionMeasurement,
            "offset",
            "The current offset of the stream processor",
            labels);

        RuntimeMetrics.Meter.CreateObservableCounter(
            $"dolittle_customer_runtime_stream_processors_processed_total",
            GetProcessedEventsMeasurement,
            "count",
            "The total number of events processed by the stream processor",
            labels);

        if(_eventLogOffsets.TryGetValue(tenant, out var getEventLogPosition))
        {
            RuntimeMetrics.Meter.CreateObservableCounter(
                $"dolittle_customer_runtime_stream_processor_consumer_lag",
                () =>
                {
                    var processorOffset = (long)getNextProcessingPosition().EventLogPosition.Value-1;
                    var eventLogPosition = getEventLogPosition();
                    var lag = eventLogPosition - processorOffset;
                    if(lag < 0) lag = 0; // Negative lag is not possible
                    
                    return new Measurement<long>(lag, labels);
                },
                "offset",
                "The current offset of the event log",
                labels);
        }

        _streamProcessorEventLogOffsets.Add(key);
        
        // The reported value is the next event log position to be processed, so we subtract 1 to get the current position
        Measurement<long> GetEventLogPositionMeasurement() => new((long)getNextProcessingPosition().EventLogPosition.Value-1, labels);
        Measurement<long> GetProcessedEventsMeasurement() => new((long)getNextProcessingPosition().StreamPosition.Value, labels);
    }

    public void RegisterEventLogOffset(TenantId tenant, ScopeId scopeId, Func<EventLogSequenceNumber> getEventLogPosition)
    {
        var key = tenant;
        if (!_eventLogOffsets.ContainsKey(key))
        {
            var getValue = () => (long)getEventLogPosition().Value;
            
            var labels = new[]
            {
                new KeyValuePair<string, object?>("tenantId", tenant.ToString()),
                new KeyValuePair<string, object?>("scopeId", scopeId.ToString()),
            };
            Measurement<long> GetMeasurement()
            {
                return new Measurement<long>(getValue(), labels);
            }

            RuntimeMetrics.Meter.CreateObservableCounter(
                $"dolittle_system_runtime_events_store_offset",
                GetMeasurement,
                "offset",
                "The current offset of the event log",
                labels);
            _eventLogOffsets.TryAdd(key, getValue);
        }
    }
}
