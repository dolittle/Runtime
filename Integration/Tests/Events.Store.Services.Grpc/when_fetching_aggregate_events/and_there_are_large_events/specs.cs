// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Integration.Shared;
using Integration.Tests.Events.Store.given;
using Machine.Specifications;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;

namespace Integration.Tests.Events.Store.Services.Grpc.when_fetching_aggregate_events.and_there_are_large_events;

class specs : given.a_clean_event_store_and_grpc_service
{
    const int MAX_BATCH_SIZE = 2097152; // 2MB
    const int APPROXIMATE_MAX_EVENT_SIZE = MAX_BATCH_SIZE - 200;
    static ArtifactId aggregate_root_id;
    static EventSourceId event_source;
    
    Establish context = () =>
    {
        aggregate_root_id = "09f05cd2-a855-4a49-ba60-20086f8d001b";
        event_source = "big event source";
    };
    
    
    [Tags("IntegrationTest")]
    class when_fetching_three_large_events_without_batching
    {
        static FetchForAggregateRequest request;
        static FetchForAggregateResponse response;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                event_to_commit.with_large_content(APPROXIMATE_MAX_EVENT_SIZE).build(),
                event_to_commit.with_large_content(APPROXIMATE_MAX_EVENT_SIZE).build(),
                event_to_commit.with_large_content(APPROXIMATE_MAX_EVENT_SIZE).build(),
            }.ToList()), execution_context).GetAwaiter().GetResult();

            request = EventStoreRequests.FetchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => response = event_store_service.FetchForAggregate(request, server_call_context).GetAwaiter().GetResult();
        
        It should_not_fail = () => response.Failure.Should().BeNull();
        It should_return_the_correct_aggregate_root = () => response.Events.AggregateRootId.ToGuid().Should().Be(aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => response.Events.CurrentAggregateRootVersion.Should().Be(3U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => response.Events.AggregateRootVersion.Should().Be(2U);
        It should_return_the_correct_event_source = () => response.Events.EventSourceId.Should().Be(event_source.Value);
        It should_return_all_events = () => response.Events.Events.Count.Should().Be(3);
    }
    
    [Tags("IntegrationTest")]
    class when_fetching_three_large_events_with_batching
    {
        static FetchForAggregateInBatchesRequest request;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                event_to_commit.with_large_content(APPROXIMATE_MAX_EVENT_SIZE).build(),
                event_to_commit.with_large_content(APPROXIMATE_MAX_EVENT_SIZE).build(),
                event_to_commit.with_large_content(APPROXIMATE_MAX_EVENT_SIZE).build(),
            }.ToList()), execution_context).GetAwaiter().GetResult();

            request = EventStoreRequests.FetchBatchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => event_store_service.FetchForAggregateInBatches(request, fetch_for_aggregate_response_stream, server_call_context).GetAwaiter().GetResult();

        It should_not_fail = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Failure == default);
        It should_write_three_batches = () => fetch_for_aggregate_written_responses.Count.Should().Be(3);
        It should_return_the_correct_aggregate_root = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.AggregateRootId.ToGuid() == aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.CurrentAggregateRootVersion == 3U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.AggregateRootVersion == 2U);
        It should_return_the_correct_event_source = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.EventSourceId == event_source.Value);
        It should_return_three_batches_with_one_event = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.Events.Count == 1);
    }
    
    
    
    [Tags("IntegrationTest")]
    class when_fetching_two_events_that_are_too_large_with_batching
    {
        static FetchForAggregateInBatchesRequest request;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                event_to_commit.with_large_content(2 * MAX_BATCH_SIZE).build(),
                event_to_commit.with_large_content(2 * MAX_BATCH_SIZE).build(),
            }.ToList()), execution_context).GetAwaiter().GetResult();

            request = EventStoreRequests.FetchBatchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => event_store_service.FetchForAggregateInBatches(request, fetch_for_aggregate_response_stream, server_call_context).GetAwaiter().GetResult();

        It should_not_fail = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Failure == default);
        It should_write_two_batches = () => fetch_for_aggregate_written_responses.Count.Should().Be(2);
        It should_return_the_correct_aggregate_root = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.AggregateRootId.ToGuid() == aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.CurrentAggregateRootVersion == 2U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.AggregateRootVersion == 1U);
        It should_return_the_correct_event_source = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.EventSourceId == event_source.Value);
        It should_return_two_batches_with_one_event = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Events.Events.Count == 1);
    }
}