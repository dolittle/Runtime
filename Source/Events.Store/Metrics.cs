// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetrics"/>.
    /// </summary>
    [Singleton]
    public class Metrics : IMetrics, ICanProvideMetrics
    {
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Metrics"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working the <see cref="ExecutionContext"/>.</param>
        public Metrics(IExecutionContextManager executionContextManager)
        {
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc/>
        public Counter FailedUncommittedEvents { get; private set; }

        /// <inheritdoc/>
        public Counter FailedUncommittedAggregateEvents { get; private set; }

        /// <inheritdoc/>
        public Counter CommittedEvents { get; private set; }

        /// <inheritdoc/>
        public Counter CommittedAggregateEvents { get; private set; }

        /// <inheritdoc/>
        public void IncrementCommittedEvents(CommittedEvent @event)
        {
            CommittedAggregateEvents.WithLabels(
                @event.ExecutionContext.Tenant.Value.ToString(),
                @event.EventSource.Value.ToString(),
                @event.Type.Id.Value.ToString(),
                @event.Public.ToString(CultureInfo.InvariantCulture)).Inc();
        }

        /// <inheritdoc/>
        public void IncrementFailedUncommittedEvents(UncommittedEvents events)
        {
            events.ForEach(@event =>
            {
                FailedUncommittedEvents.WithLabels(
                    _executionContextManager.Current.Tenant.Value.ToString(),
                    @event.Type.Id.Value.ToString(),
                    @event.Public.ToString(CultureInfo.InvariantCulture)).Inc();
            });
        }

        /// <inheritdoc/>
        public void IncrementCommittedAggregateEvents(CommittedAggregateEvent @event)
        {
            CommittedAggregateEvents.WithLabels(
                @event.ExecutionContext.Tenant.Value.ToString(),
                @event.EventSource.Value.ToString(),
                @event.AggregateRoot.Id.Value.ToString(),
                @event.Type.Id.Value.ToString(),
                @event.Public.ToString(CultureInfo.InvariantCulture)).Inc();
        }

        /// <inheritdoc/>
        public void IncrementFailedUncommittedAggregateEvents(UncommittedAggregateEvents events)
        {
            events.ForEach(@event =>
            {
                FailedUncommittedAggregateEvents.WithLabels(
                    _executionContextManager.Current.Tenant.Value.ToString(),
                    events.EventSource.Value.ToString(),
                    events.AggregateRoot.Id.Value.ToString(),
                    @event.Type.Id.Value.ToString(),
                    @event.Public.ToString(CultureInfo.InvariantCulture)).Inc();
            });
        }

        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            FailedUncommittedEvents = metricFactory.Counter("FailedCommittedEvents", "Number of failed uncommitted events", "TenantId", "EventSourceId", "EventArtifactId", "Public");
            FailedUncommittedAggregateEvents = metricFactory.Counter("FailedUncommittedAggregateEvents", "Number of failed uncommitted aggregate events", "TenantId", "EventSourceId", "AggregateArtifactId", "EventArtifactId", "Public");
            CommittedEvents = metricFactory.Counter("CommittedEvents", "Number of committed events", "TenantId", "EventSourceId", "EventArtifactId", "Public");
            CommittedAggregateEvents = metricFactory.Counter("CommittedAggregateEvents", "Number of committed aggregate events", "TenantId", "EventSourceId", "AggregateArtifactId", "EventArtifactId", "Public");

            return new Collector[]
            {
                FailedUncommittedEvents,
                FailedUncommittedAggregateEvents,
                CommittedEvents,
                CommittedAggregateEvents
            };
        }
    }
}