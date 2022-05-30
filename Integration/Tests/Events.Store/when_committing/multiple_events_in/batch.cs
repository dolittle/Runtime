// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using CommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.when_committing.multiple_events_in;

class batch : given.a_clean_event_store
{
    static List<UncommittedEvent> uncommitted_events_list;
    static List<UncommittedEvent> uncommitted_events_list_after_subscribe;
    static EventSourceId event_source;

    Establish context = () =>
    {
        event_source = "some event source";
        uncommitted_events_list = new List<UncommittedEvent>
        {
            given.event_to_commit.with_content(new { Hello = "world" }).with_event_source(event_source).build(),
            given.event_to_commit.with_content(new { Hello = "Jakob" }).with_event_source(event_source).build(),
            given.event_to_commit.with_content(new { Hello = "Groot" }).with_event_source(event_source).build(),
        };

        uncommitted_events_list_after_subscribe = Enumerable.Range(0, 2000)
            .Select(i => given.event_to_commit.with_content(new { Hello = $"event_{i}" }).with_event_source(event_source).build()).ToList();
    };

    [Tags("IntegrationTest")]
    class not_for_aggregate
    {
        static UncommittedEvents uncommitted_events;
        static UncommittedEvents uncommitted_events_after_subscribe;
        static CommitEventsResponse response_before_subscribe;
        static CommitEventsResponse response_after_subscribe;
        static List<EventLogBatch> all_subscription_batch_results;
        static List<CommittedEvent> all_committed_events;

        Establish context = () =>
        {
            uncommitted_events = new UncommittedEvents(uncommitted_events_list);
            uncommitted_events_after_subscribe = new UncommittedEvents(uncommitted_events_list_after_subscribe);
        };

        Because of = () =>
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            response_before_subscribe = event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
            var subscribedEvents = event_log_stream.SubscribeAll(ScopeId.Default, 0, cts.Token).ReadAllAsync().ToListAsync();
            response_after_subscribe = event_store.Commit(uncommitted_events_after_subscribe, execution_context).GetAwaiter().GetResult();
            all_committed_events = response_before_subscribe.Events.Concat(response_after_subscribe.Events).ToList();
            all_subscription_batch_results = subscribedEvents.GetAwaiter().GetResult();
        };

        It should_not_fail = () => response_before_subscribe.Failure.ShouldBeNull();

        It should_return_the_correct_committed_event = () =>
            response_before_subscribe.should_be_the_correct_response(uncommitted_events, execution_context, EventLogSequenceNumber.Initial);

        It should_have_stored_events_in_the_event_log = () => number_of_events_stored_should_be(all_committed_events.Count);

        It should_have_stored_the_correct_events =
            () => all_committed_events.ToCommittedEvents().should_have_stored_committed_events(streams, event_content_converter);

        It should_have_received_all_events = () =>
        {
            var all_subscribed_events = all_subscription_batch_results.SelectMany(it => it.MatchedEvents).ToList();
            all_subscribed_events.should_be_the_same_committed_events(all_committed_events);
        };
        
        It first_batch_gets_catchup_events = () =>
        {
            var first_batch = all_subscription_batch_results.First();
            first_batch.From.Value.ShouldEqual((ulong)0);
            first_batch.To.Value.ShouldEqual((ulong)2);
            first_batch.MatchedEvents.should_be_the_same_committed_events(response_before_subscribe.Events);
        };
        
        It next_batch_gets_live_events = () =>
        {
            var second_batch = all_subscription_batch_results[1];
            second_batch.From.Value.ShouldEqual((ulong)3);
            var committedEvents = second_batch.MatchedEvents;
            committedEvents.should_be_the_same_committed_events(response_after_subscribe.Events.Take(second_batch.MatchedEvents.Count).ToList());
        };
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

        It should_return_the_correct_committed_event = () => response.should_be_the_correct_response(uncommitted_events, execution_context,
            EventLogSequenceNumber.Initial, uncommitted_events.ExpectedAggregateRootVersion);

        It should_have_stored_one_event_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Count);

        It should_have_stored_the_correct_events =
            () => response.Events.ToCommittedEvents().should_have_stored_committed_events(streams, event_content_converter);
    }
}