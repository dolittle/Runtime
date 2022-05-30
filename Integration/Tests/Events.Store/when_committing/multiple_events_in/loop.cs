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
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.when_committing.multiple_events_in;

class loop : given.a_clean_event_store
{
    static IEnumerable<List<UncommittedEvent>> events_to_commit;
    static EventSourceId event_source;

    Establish context = () =>
    {
        event_source = "some event source";
        events_to_commit = new []
        {
            new List<UncommittedEvent>
            {
                given.event_to_commit.with_content(new {Hello = "world"}).with_event_source(event_source).build()
            },
            new List<UncommittedEvent>
            {
                given.event_to_commit.with_content(new {Hello = "Jakob"}).with_event_source(event_source).build(),
            },
            new List<UncommittedEvent>
            {
                given.event_to_commit.with_content(new {Hello = "Groot"}).with_event_source(event_source).build(),
            }
        };
    };
    
    [Tags("IntegrationTest")]
    class not_for_aggregate
    {
        static UncommittedEvents[] uncommitted_events;
        static List<CommitEventsResponse> responses = new();

        Establish context = () =>
        {
            uncommitted_events = events_to_commit.Select(_ => new UncommittedEvents(_)).ToArray();
        };
        
        Because of = () =>
        {
            foreach (var events in uncommitted_events)
            {
                responses.Add(event_store.Commit(events, execution_context).GetAwaiter().GetResult());
            }
        };

        It should_not_fail = () => responses.ShouldEachConformTo(_ => _.Failure == default);

        It should_return_the_correct_committed_event = () => responses.ToArray().should_be_the_correct_responses(uncommitted_events, execution_context);

        It should_have_stored_one_event_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Sum(_ => _.Count));

        It should_have_stored_the_correct_events = () => responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
        
        It should_have_subscribable_events  = () =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var reader = event_log_stream.SubscribeAll(ScopeId.Default, 0, cts.Token);
            var hasData = reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult();
            hasData.ShouldBeTrue(); // Should get a single batch

            reader.TryRead(out var batch);
            var events = responses.Select(_ => _.Events.ToCommittedEvents()).SelectMany(it => it).ToList();
            var batchCommittedEvents = batch!.MatchedEvents.ToCommittedEvents().ToList();

            batchCommittedEvents.should_be_the_same_committed_events(events);
        };
    }
    
    [Tags("IntegrationTest")]
    class for_an_aggregate
    {
        static UncommittedAggregateEvents[] uncommitted_events; 
        static List<CommitAggregateEventsResponse> responses = new();
        static Artifact aggregate_root;
        
        Establish context = () =>
        {
            var expectedAggregateRootVersion = AggregateRootVersion.Initial;
            aggregate_root = new Artifact("8640c18a-8d3c-48bb-959f-bb894907b9aa", ArtifactGeneration.First);
            uncommitted_events = events_to_commit.Select(_ =>
            {
                var events = new UncommittedAggregateEvents(
                    event_source,
                    aggregate_root,
                    expectedAggregateRootVersion,
                    _
                );
                expectedAggregateRootVersion += (ulong) _.Count;
                return events;
            }).ToArray();
        };
        
        Because of = () =>
        {
            foreach (var events in uncommitted_events)
            {
                responses.Add(event_store.Commit(events, execution_context).GetAwaiter().GetResult());
            }
        };

        It should_not_fail = () => responses.ShouldEachConformTo(_ => _.Failure == default);

        It should_return_the_correct_committed_event = () => responses.ToArray().should_be_the_correct_responses(uncommitted_events, execution_context);

        It should_have_stored_one_event_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Sum(_ => _.Count));

        It should_have_stored_the_correct_events = () => responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
    }
}