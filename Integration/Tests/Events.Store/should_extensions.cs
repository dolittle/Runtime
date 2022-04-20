// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Execution;
using Machine.Specifications;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using CommittedAggregateEvents = Dolittle.Runtime.Events.Store.CommittedAggregateEvents;
using CommittedEvent = Dolittle.Runtime.Events.Store.CommittedEvent;
using Event = Dolittle.Runtime.Events.Store.MongoDB.Events.Event;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Integration.Tests.Events.Store;

static class should_extensions
{

    public static void should_be_the_correct_responses(this CommitEventsResponse[] responses, UncommittedEvents[] uncommitted_events, ExecutionContext execution_context, bool random_commit_order = false)
    {
        var startEventLogSequenceNumber = EventLogSequenceNumber.Initial;
        for (var i = 0; i < responses.Length; i++)
        {
            var response = responses[i];
            var events = uncommitted_events[i];
            response.should_be_the_correct_response(events, execution_context, random_commit_order ? response.Events?.First().EventLogSequenceNumber : startEventLogSequenceNumber);
            startEventLogSequenceNumber += (ulong)events.Count;
        }
    }
    public static void should_be_the_correct_responses(this CommitAggregateEventsResponse[] responses, UncommittedAggregateEvents[] uncommitted_events, ExecutionContext execution_context, bool random_commit_order = false)
    {
        var startEventLogSequenceNumber = EventLogSequenceNumber.Initial;
        for (var i = 0; i < responses.Length; i++)
        {
            var response = responses[i];
            var events = uncommitted_events[i];
            response.should_be_the_correct_response(events, execution_context, random_commit_order ? response.Events?.Events?.First().EventLogSequenceNumber : startEventLogSequenceNumber);
            startEventLogSequenceNumber += (ulong)events.Count;
        }
    }
    
    public static void should_be_the_correct_response(this CommitEventsResponse response, UncommittedEvents uncommitted_events, ExecutionContext execution_context, EventLogSequenceNumber start_sequence_number = null)
    {
        var committedEvents = response.Events.ToCommittedEvents();

        committedEvents.Count.ShouldEqual(uncommitted_events.Count);
        if (uncommitted_events.Count == 0)
        {
            return;
        }

        for (var i = 0; i < committedEvents.Count; i++)
        {
            var committedEvent = committedEvents[i];
            var uncommittedEvent = uncommitted_events[i];

            if (start_sequence_number != null)
            {
                committedEvent.EventLogSequenceNumber.ShouldEqual(new EventLogSequenceNumber(start_sequence_number + (ulong)i));
            }
            
            committedEvent.ExecutionContext.ShouldEqual(execution_context);
            committedEvent.Content.ShouldEqual(uncommittedEvent.Content);
            committedEvent.Public.ShouldEqual(uncommittedEvent.Public);
            committedEvent.Type.ShouldEqual(uncommittedEvent.Type);
            committedEvent.EventSource.ShouldEqual(uncommittedEvent.EventSource);
        }
    }
    public static void should_be_the_correct_response(
        this CommitAggregateEventsResponse response,
        UncommittedAggregateEvents uncommitted_events,
        ExecutionContext execution_context,
        EventLogSequenceNumber start_sequence_number = null,
        AggregateRootVersion start_aggregate_root_version = null)
    {
        var committedEvents = response.Events.ToCommittedEvents();

        committedEvents.Count.ShouldEqual(uncommitted_events.Count);
        if (uncommitted_events.Count == 0)
        {
            return;
        }

        committedEvents.AggregateRoot.ShouldEqual(uncommitted_events.AggregateRoot.Id);
        committedEvents.EventSource.ShouldEqual(uncommitted_events.EventSource);

        for (var i = 0; i < committedEvents.Count; i++)
        {
            var committedEvent = committedEvents[i];
            var uncommittedEvent = uncommitted_events[i];

            if (start_sequence_number != null)
            {
                committedEvent.EventLogSequenceNumber.ShouldEqual(new EventLogSequenceNumber(start_sequence_number + (ulong)i));
            }
            if (start_aggregate_root_version != null)
            {
                committedEvent.AggregateRootVersion.ShouldEqual(new AggregateRootVersion(start_aggregate_root_version + (ulong)i));
            }

            committedEvent.AggregateRoot.ShouldEqual(uncommitted_events.AggregateRoot);
            committedEvent.ExecutionContext.ShouldEqual(execution_context);
            committedEvent.Content.ShouldEqual(uncommittedEvent.Content);
            committedEvent.Public.ShouldEqual(uncommittedEvent.Public);
            committedEvent.Type.ShouldEqual(uncommittedEvent.Type);
            committedEvent.EventSource.ShouldEqual(uncommittedEvent.EventSource);
        }
    }
    
    public static void should_have_stored_committed_events<TEvent>(this CommittedEventSequence<TEvent> events, IStreams streams, IEventContentConverter event_content_converter)
        where TEvent : Dolittle.Runtime.Events.Store.CommittedEvent
    {
        var eventLog = streams.DefaultEventLog;
        var storedEvents = eventLog.FindSync(Builders<Event>.Filter.Empty).ToList();

        switch (events)
        {
            case CommittedEvents committedEvents:
                should_have_stored_committed_events(event_content_converter, committedEvents, storedEvents);
                break;
            case CommittedAggregateEvents committedAggregateEvents:
                should_have_stored_committed_events(event_content_converter, committedAggregateEvents, storedEvents);
                break;
            default:
                throw new Exception("Wrong committed events type");
        }
    }
    
    public static void should_have_stored_committed_events<TEvent>(
        this IEnumerable<CommittedEventSequence<TEvent>> batches,
        IStreams streams,
        IEventContentConverter event_content_converter)
        where TEvent : Dolittle.Runtime.Events.Store.CommittedEvent
    {
        var eventLog = streams.DefaultEventLog;

        foreach (var batch in batches)
        {
            var storedEvents = eventLog.FindSync(
                Builders<Event>.Filter.Gte(_ => _.EventLogSequenceNumber, batch.First().EventLogSequenceNumber.Value)
                & Builders<Event>.Filter.Lte(_ => _.EventLogSequenceNumber, batch.Last().EventLogSequenceNumber.Value)).ToList();
            switch (batch)
            {
                case CommittedEvents committedEvents:
                    should_have_stored_committed_events(event_content_converter, committedEvents, storedEvents);
                    break;
                case CommittedAggregateEvents committedAggregateEvents:
                    should_have_stored_committed_events(event_content_converter, committedAggregateEvents, storedEvents);
                    break;
                default:
                    throw new Exception("Wrong committed events type");
            }
        }
    }
    static void should_have_stored_committed_events(IEventContentConverter event_content_converter, CommittedAggregateEvents aggregate_events, List<Event> stored_events)
    {
        stored_events.Count.ShouldEqual(aggregate_events.Count);
        if (stored_events.Count == 0)
        {
            return;
        }
        stored_events.ShouldEachConformTo(_ => _.Aggregate.WasAppliedByAggregate == true);
        stored_events.ShouldEachConformTo(_ => _.Aggregate.TypeGeneration.Equals(ArtifactGeneration.First.Value));
        stored_events.ShouldEachConformTo(_ => _.Aggregate.TypeId.Equals(aggregate_events.AggregateRoot.Value));
        stored_events.ShouldEachConformTo(_ => _.Metadata.EventSource.Equals(aggregate_events.EventSource));
        stored_events.ShouldEachConformTo(_ => _.EventHorizon.FromEventHorizon == false);

        for (var i = 0; i < stored_events.Count; i++)
        {
            var storedEvent = stored_events[i];
            var committedEvent = aggregate_events[i];

            storedEvent.Aggregate.Version.ShouldEqual(committedEvent.AggregateRootVersion.Value);
            should_be_the_same_base_committed_event(event_content_converter, committedEvent, storedEvent);
        }
    }
    
    static void should_have_stored_committed_events(IEventContentConverter event_content_converter, CommittedEvents events, List<Event> stored_events)
    {
        stored_events.Count.ShouldEqual(events.Count);
        if (stored_events.Count == 0)
        {
            return;
        }
        stored_events.ShouldEachConformTo(_ => _.Aggregate.WasAppliedByAggregate == false);
        stored_events.ShouldEachConformTo(_ => _.EventHorizon.FromEventHorizon == false);
        
        for (var i = 0; i < stored_events.Count; i++)
        {
            should_be_the_same_base_committed_event(event_content_converter, events[i], stored_events[i]);
        }
    }

    static void should_have_same_execution_context(ExecutionContext execution_context, Dolittle.Runtime.Events.Store.MongoDB.Events.ExecutionContext stored_execution_context)
    {
        stored_execution_context.Correlation.ShouldEqual(execution_context.CorrelationId.Value);
        stored_execution_context.Environment.ShouldEqual(execution_context.Environment.Value);
        stored_execution_context.Microservice.ShouldEqual(execution_context.Microservice.Value);
        stored_execution_context.Tenant.ShouldEqual(execution_context.Tenant.Value);
        should_have_the_same_version(execution_context.Version, stored_execution_context.Version);
        should_have_the_same_claims(execution_context.Claims, stored_execution_context.Claims);
    }
    
    static void should_be_the_same_base_committed_event(IEventContentConverter event_content_converter, CommittedEvent committed_event, Event stored_event)
    {
        JToken.DeepEquals(JToken.Parse(event_content_converter.ToJson(stored_event.Content)), JToken.Parse(committed_event.Content)).ShouldBeTrue();
        stored_event.Metadata.Occurred.ShouldBeCloseTo(committed_event.Occurred.UtcDateTime, TimeSpan.FromSeconds(1));
        stored_event.Metadata.Public.ShouldEqual(committed_event.Public);
        stored_event.Metadata.EventSource.ShouldEqual(committed_event.EventSource.Value);
        stored_event.Metadata.TypeGeneration.ShouldEqual(committed_event.Type.Generation.Value);
        stored_event.Metadata.TypeId.ShouldEqual(committed_event.Type.Id.Value);
        should_have_same_execution_context(committed_event.ExecutionContext, stored_event.ExecutionContext);
        stored_event.EventLogSequenceNumber.ShouldEqual(committed_event.EventLogSequenceNumber.Value);
    }

    static void should_have_the_same_version(Version version, Dolittle.Runtime.Events.Store.MongoDB.Events.Version stored_version)
    {
        version.Build.ShouldEqual(stored_version.Build);
        version.Major.ShouldEqual(stored_version.Major);
        version.Minor.ShouldEqual(stored_version.Minor);
        version.Patch.ShouldEqual(stored_version.Patch);
        version.PreReleaseString.ShouldEqual(stored_version.PreRelease);
    }
    static void should_have_the_same_claims(Claims claims, IEnumerable<Dolittle.Runtime.Events.Store.MongoDB.Events.Claim> stored_claims)
    {
        stored_claims.Count().ShouldEqual(claims.Count());
        foreach (var claim in claims)
        {
            stored_claims.Any(stored_claim => stored_claim.Name.Equals(claim.Name)
                && stored_claim.Value.Equals(claim.Value)
                && stored_claim.ValueType.Equals(claim.ValueType)).ShouldBeTrue();
        }
    }
}