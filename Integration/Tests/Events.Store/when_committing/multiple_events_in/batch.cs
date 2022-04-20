// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.when_committing.multiple_events_in;

class batch : given.a_clean_event_store
{
    static List<UncommittedEvent> uncommitted_events_list;
    static EventSourceId event_source;

    Establish context = () =>
    {
        event_source = "some event source";
        uncommitted_events_list = new List<UncommittedEvent>
        {
            given.event_to_commit.with_content(new {Hello = "world"}).with_event_source(event_source).build(),
            given.event_to_commit.with_content(new {Hello = "Jakob"}).with_event_source(event_source).build(),
            given.event_to_commit.with_content(new {Hello = "Groot"}).with_event_source(event_source).build(),
        };
    };
    
    [Tags("IntegrationTest")]
    class not_for_aggregate
    {
        static UncommittedEvents uncommitted_events;
        static CommitEventsResponse response;

        Establish context = () => uncommitted_events = new UncommittedEvents(uncommitted_events_list);
        
        Because of = () => response = event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();

        It should_not_fail = () => response.Failure.ShouldBeNull();

        It should_return_the_correct_committed_event = () => response.should_be_the_correct_response(uncommitted_events, execution_context, EventLogSequenceNumber.Initial);

        It should_have_stored_one_event_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Count);

        It should_have_stored_the_correct_events = () => response.Events.ToCommittedEvents().should_have_stored_committed_events(streams, event_content_converter);
    }
    
    [Tags("IntegrationTest")]
    class for_an_aggregate
    {
        static UncommittedAggregateEvents uncommitted_events; 
        static CommitAggregateEventsResponse response;
        static Artifact aggregate_root;

        Establish context = () =>
        {
            aggregate_root = new Artifact("8640c18a-8d3c-48bb-959f-bb894907b9aa", ArtifactGeneration.First);
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                aggregate_root,
                AggregateRootVersion.Initial,
                uncommitted_events_list);
        };

        Because of = () => response = event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();

        It should_not_fail = () => response.Failure.ShouldBeNull();

        It should_return_the_correct_committed_event = () => response.should_be_the_correct_response(uncommitted_events, execution_context, EventLogSequenceNumber.Initial, uncommitted_events.ExpectedAggregateRootVersion);

        It should_have_stored_one_event_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Count);
        
        It should_have_stored_the_correct_events = () => response.Events.ToCommittedEvents().should_have_stored_committed_events(streams, event_content_converter);
    }
}