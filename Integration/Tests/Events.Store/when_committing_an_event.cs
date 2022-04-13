// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using MongoDB.Driver;
using Event = Dolittle.Runtime.Events.Store.MongoDB.Events.Event;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store;

class when_committing_an_event : given.a_clean_event_store
{
    static UncommittedEvent event_to_commit;
    static List<UncommittedEvent> uncommitted_events_list;
    static EventSourceId event_source;
    
    Establish context = () =>
    {
        event_source = "some event source";
        event_to_commit = new UncommittedEvent(event_source, Artifact.New(), false, "{\"hello\": 42}");
        uncommitted_events_list = new List<UncommittedEvent>
        {
            event_to_commit
        };
    };
    
    [Tags("IntegrationTest")]
    class once
    {
        static CommitEventsResponse response;

        Because of = () => response = event_store.Commit(new UncommittedEvents(uncommitted_events_list), execution_context).GetAwaiter().GetResult();

        It should_not_fail = () => response.Failure.ShouldBeNull();

        It should_return_the_correct_committed_event = () =>
        {
            var committedEvents = response.Events.ToCommittedEvents();

            committedEvents.Count.ShouldEqual(1);
            var committedEvent = committedEvents.First();
            committedEvent.ExecutionContext.ShouldEqual(execution_context);
            committedEvent.EventLogSequenceNumber.ShouldEqual(new EventLogSequenceNumber(0));
            committedEvent.Content.ShouldEqual(event_to_commit.Content);
            committedEvent.Public.ShouldEqual(event_to_commit.Public);
            committedEvent.Type.ShouldEqual(event_to_commit.Type);
            committedEvent.EventSource.ShouldEqual(event_to_commit.EventSource);
        };

        It should_have_stored_one_event_in_the_event_log = () => streams.DefaultEventLog.CountDocuments(Builders<Event>.Filter.Empty).ShouldEqual(1);
    }
    
    [Tags("IntegrationTest")]
    class for_an_aggregate
    {
        static CommitAggregateEventsResponse response;
        static Artifact aggregate_root;

        Establish context = () =>
        {
            aggregate_root = new Artifact("8640c18a-8d3c-48bb-959f-bb894907b9aa", ArtifactGeneration.First);
        };

        Because of = () => response = event_store.Commit(new UncommittedAggregateEvents(
            event_source,
            aggregate_root,
            0,
            uncommitted_events_list),execution_context).GetAwaiter().GetResult();

        It should_not_fail = () => response.Failure.ShouldBeNull();

        It should_return_the_correct_committed_event = () =>
        {
            var committedEvents = response.Events.ToCommittedEvents();

            committedEvents.Count.ShouldEqual(1);
            var committedEvent = committedEvents.First();
            committedEvent.ExecutionContext.ShouldEqual(execution_context);
            committedEvent.EventLogSequenceNumber.ShouldEqual(new EventLogSequenceNumber(0));
            committedEvent.Content.ShouldEqual(event_to_commit.Content);
            committedEvent.Public.ShouldEqual(event_to_commit.Public);
            committedEvent.Type.ShouldEqual(event_to_commit.Type);
            committedEvent.EventSource.ShouldEqual(event_to_commit.EventSource);
        };

        It should_have_stored_one_event_in_the_event_log = () => streams.DefaultEventLog.CountDocuments(Builders<Event>.Filter.Empty).ShouldEqual(1);
    }
}