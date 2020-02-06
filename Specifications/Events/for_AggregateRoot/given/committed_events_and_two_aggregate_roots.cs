// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Events;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot.given
{
    public abstract class committed_events_and_two_aggregate_roots : two_aggregate_roots
    {
        static CorrelationId correlationId = Guid.Parse("2105cf5d-3134-41c3-9617-5a9e8c833ca4");
        static Microservice microserviceId = Guid.Parse("9624658b-8caf-44b8-b891-7f53f69f8b5e");
        static TenantId tenantId = Guid.Parse("54ad514b-baa5-44f5-8a6b-870d2ce0dcb2");
        static Cause cause = new Cause(CauseType.Command, 0);

        public static CommittedAggregateEvents build_committed_events(EventSourceId eventSource, Type aggregateRoot)
        {
            var events = new List<CommittedAggregateEvent>();
            events.Add(build_committed_event(
                eventSource,
                aggregateRoot,
                0,
                0,
                event_one));
            events.Add(build_committed_event(
                eventSource,
                aggregateRoot,
                1,
                1,
                event_two));
            events.Add(build_committed_event(
                eventSource,
                aggregateRoot,
                2,
                2,
                event_three));
            return new CommittedAggregateEvents(eventSource, aggregateRoot, 3, events);
        }

        static CommittedAggregateEvent build_committed_event(EventSourceId eventSource, Type aggregateRoot, AggregateRootVersion aggregateRootVersion, EventLogVersion eventLogVersion, IEvent @event)
        {
            return new CommittedAggregateEvent(
                eventSource,
                aggregateRoot,
                aggregateRootVersion,
                eventLogVersion,
                DateTimeOffset.Now,
                correlationId,
                microserviceId,
                tenantId,
                cause,
                @event);
        }
    }
}
