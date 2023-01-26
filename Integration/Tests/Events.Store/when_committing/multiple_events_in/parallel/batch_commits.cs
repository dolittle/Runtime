// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using FluentAssertions;
using Machine.Specifications;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.when_committing.multiple_events_in.parallel;

class batch_commits : given.a_clean_event_store
{
    static int number_of_parallel_committers;
    static int batch_size;

    Establish context = () =>
    {
        number_of_parallel_committers = 10;
        batch_size = 10;
    };

    static List<UncommittedEvent> create_uncommitted_events(EventSourceId event_source)
        => Enumerable
            .Range(0, batch_size)
            .Select(event_num => given.event_to_commit
                .with_content(new
                {
                    Hello = $"Event {event_num} from {event_source}"
                })
                .with_event_source(event_source)
                .build()).ToList();

    [Tags("IntegrationTest")]
    class not_for_aggregate
    {
        static UncommittedEvents[] uncommitted_events;
        static CommitEventsResponse[] responses;

        Establish context = () =>
        {
            uncommitted_events = Enumerable.Range(0, number_of_parallel_committers).Select(commiter => new UncommittedEvents(
                create_uncommitted_events($"Committer {commiter}"))).ToArray();
        };
        
        Because of = () =>
        {
            responses = Task.WhenAll(uncommitted_events.Select(_ => event_store.Commit(_, execution_context))).GetAwaiter().GetResult();
        };

        It should_not_fail = () => responses.ShouldEachConformTo(_ => _.Failure == default);

        It should_return_the_correct_committed_event = () => responses.ToArray().should_be_the_correct_responses(uncommitted_events, execution_context, random_commit_order: true);

        It should_have_stored_the_correct_number_of_events_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Sum(_ => _.Count));

        It should_have_stored_the_correct_events = () => responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
    }
    
    [Tags("IntegrationTest")]
    class for_a_single_aggregate
    {
        static UncommittedAggregateEvents[] uncommitted_events;
        static CommitAggregateEventsResponse[] responses;
        static EventSourceId event_source;
        static Artifact aggregate_root;

        static int number_of_failed_responses => responses.Count(_ => _.Failure != null);
        static CommitAggregateEventsResponse[] successful_responses => responses.Where(_ => _.Failure == null).ToArray();
        
        Establish context = () =>
        {
            event_source = "some aggregate";
            var expectedAggregateRootVersion = AggregateRootVersion.Initial;
            aggregate_root = new Artifact("8640c18a-8d3c-48bb-959f-bb894907b9aa", ArtifactGeneration.First);
            uncommitted_events = Enumerable.Range(0, number_of_parallel_committers).Select(committer =>
            {
                var events = new UncommittedAggregateEvents(
                    event_source,
                    aggregate_root,
                    expectedAggregateRootVersion++,
                    create_uncommitted_events(event_source));
                return events;
            }).ToArray();
        };

        Because of = () =>
        {
            responses = Task.WhenAll(uncommitted_events.Select(_ => event_store.Commit(_, execution_context))).GetAwaiter().GetResult();
        };
        
        It should_have_at_least_one_failed_commits = () => responses.Count(_ => _.Failure != default).Should().BeGreaterThanOrEqualTo(1);
        It should_not_be_able_to_commit_all_events = () => streams.DefaultEventLog.CountDocuments(all_events_filter).Should().BeLessThan(uncommitted_events.Sum(_ => _.Count));
        It should_return_the_correct_successfully_committed_event = () => successful_responses.should_be_the_correct_responses(uncommitted_events, execution_context);
        It should_have_committed_the_correct_number_of_events = () => number_of_events_stored_should_be(successful_responses.Length * batch_size);
        It should_have_stored_the_correct_events = () => successful_responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
    }
    
    [Tags("IntegrationTest")]
    class for_different_aggregates
    {
        static UncommittedAggregateEvents[] uncommitted_events;
        static CommitAggregateEventsResponse[] responses;
        static Artifact aggregate_root;

        Establish context = () =>
        {
            aggregate_root = new Artifact("8640c18a-8d3c-48bb-959f-bb894907b9aa", ArtifactGeneration.First);
            uncommitted_events = Enumerable.Range(0, number_of_parallel_committers).Select(committer =>
            {
                var event_source = $"aggregate {committer}";
                var events = new UncommittedAggregateEvents(
                    event_source,
                    aggregate_root,
                    AggregateRootVersion.Initial,
                    create_uncommitted_events(event_source));
                return events;
            }).ToArray();
        };

        Because of = () =>
        {
            responses = Task.WhenAll(uncommitted_events.Select(_ => event_store.Commit(_, execution_context))).GetAwaiter().GetResult();
        };
    

        It should_not_fail = () => responses.ShouldEachConformTo(_ => _.Failure == default);

        It should_return_the_correct_committed_event = () => responses.ToArray().should_be_the_correct_responses(uncommitted_events, execution_context, random_commit_order: true);

        It should_have_stored_the_correct_number_of_events_in_the_event_log = () => number_of_events_stored_should_be(uncommitted_events.Sum(_ => _.Count));

        It should_have_stored_the_correct_events = () => responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
    }
    
    [Tags("IntegrationTest")]
    class for_different_aggregates_and_non_aggregate_events
    {
        static UncommittedAggregateEvents[] agggregate_uncommitted_events;
        static CommitAggregateEventsResponse[] aggregate_responses;
        
        static UncommittedEvents[] non_agggregate_uncommitted_events;
        static CommitEventsResponse[] non_aggregate_responses;
        static Artifact aggregate_root;

        Establish context = () =>
        {
            aggregate_root = new Artifact("8640c18a-8d3c-48bb-959f-bb894907b9aa", ArtifactGeneration.First);
            agggregate_uncommitted_events = Enumerable.Range(0, number_of_parallel_committers).Select(committer =>
            {
                var event_source = $"aggregate {committer}";
                var events = new UncommittedAggregateEvents(
                    event_source,
                    aggregate_root,
                    AggregateRootVersion.Initial,
                    create_uncommitted_events(event_source));
                return events;
            }).ToArray();
            
            non_agggregate_uncommitted_events = Enumerable.Range(0, number_of_parallel_committers).Select(committer => new UncommittedEvents(
                create_uncommitted_events($"Committer {committer}"))).ToArray();
        };

        Because of = () =>
        {
            var aggregate_responses_task = Task.WhenAll(agggregate_uncommitted_events
                .Select(_ => event_store.Commit(_, execution_context)));
            var non_aggregate_responses_task = Task.WhenAll(non_agggregate_uncommitted_events
                .Select(_ => event_store.Commit(_, execution_context)));

            Task.WhenAll(aggregate_responses_task, non_aggregate_responses_task).GetAwaiter().GetResult();
            aggregate_responses = aggregate_responses_task.Result;
            non_aggregate_responses = non_aggregate_responses_task.Result;
        };
    

        It should_not_fail_committing_non_aggregate_events = () => non_aggregate_responses.ShouldEachConformTo(_ => _.Failure == default);
        It should_not_fail_committing_aggregate_events = () => aggregate_responses.ShouldEachConformTo(_ => _.Failure == default);

        It should_return_the_correct_committed_non_aggregate_events = () => non_aggregate_responses.ToArray().should_be_the_correct_responses(non_agggregate_uncommitted_events, execution_context, random_commit_order: true);
        It should_return_the_correct_committed_aggregate_events = () => aggregate_responses.ToArray().should_be_the_correct_responses(agggregate_uncommitted_events, execution_context, random_commit_order: true);

        It should_have_stored_the_correct_number_of_events_in_the_event_log = () => number_of_events_stored_should_be(non_agggregate_uncommitted_events.Sum(_ => _.Count) + agggregate_uncommitted_events.Sum(_ => _.Count));

        It should_have_stored_the_correct_non_aggregate_events = () => non_aggregate_responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
        It should_have_stored_the_correct_aggregate_events = () => aggregate_responses.Select(_ => _.Events.ToCommittedEvents()).should_have_stored_committed_events(
            streams,
            event_content_converter);
    }
}