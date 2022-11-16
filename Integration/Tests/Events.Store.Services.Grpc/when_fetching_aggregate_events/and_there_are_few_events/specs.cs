// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Integration.Shared;
using Integration.Tests.Events.Store.given;
using Machine.Specifications;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.Services.Grpc.when_fetching_aggregate_events.and_there_are_few_events;

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
    class when_fetching_no_events_without_batching
    {
        static FetchForAggregateRequest request;
        static FetchForAggregateResponse response;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new List<UncommittedEvent>()), execution_context).GetAwaiter().GetResult();
            request = EventStoreRequests.FetchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => response = event_store_service.FetchForAggregate(request, server_call_context).GetAwaiter().GetResult();
        
        It should_not_fail = () => response.Failure.ShouldBeNull();
        It should_return_the_correct_aggregate_root = () => response.Events.AggregateRootId.ToGuid().ShouldEqual(aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => response.Events.CurrentAggregateRootVersion.ShouldEqual(0U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => response.Events.AggregateRootVersion.ShouldEqual(0U);
        It should_return_the_correct_event_source = () => response.Events.EventSourceId.ShouldEqual(event_source.Value);
        It should_return_no_events = () => response.Events.Events.ShouldBeEmpty();
    }
    
    [Tags("IntegrationTest")]
    class when_fetching_one_event_without_batching
    {
        static FetchForAggregateRequest request;
        static FetchForAggregateResponse response;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                event_to_commit.create(),
            }.ToList()), execution_context).GetAwaiter().GetResult();
            request = EventStoreRequests.FetchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => response = event_store_service.FetchForAggregate(request, server_call_context).GetAwaiter().GetResult();
        
        It should_not_fail = () => response.Failure.ShouldBeNull();
        It should_return_the_correct_aggregate_root = () => response.Events.AggregateRootId.ToGuid().ShouldEqual(aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => response.Events.CurrentAggregateRootVersion.ShouldEqual(1U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => response.Events.AggregateRootVersion.ShouldEqual(0U);
        It should_return_the_correct_event_source = () => response.Events.EventSourceId.ShouldEqual(event_source.Value);
        It should_return_one_event = () => response.Events.Events.Count.ShouldEqual(1);
    }
    
    [Tags("IntegrationTest")]
    class when_fetching_no_events_with_batching
    {
        static FetchForAggregateInBatchesRequest request;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new List<UncommittedEvent>()), execution_context).GetAwaiter().GetResult();
            request = EventStoreRequests.FetchBatchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => event_store_service.FetchForAggregateInBatches(request, fetch_for_aggregate_response_stream, server_call_context).GetAwaiter().GetResult();

        It should_not_fail = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Failure == default);
        It should_write_one_batch = () => fetch_for_aggregate_written_responses.Count.ShouldEqual(1);
        It should_return_the_correct_aggregate_root = () => fetch_for_aggregate_written_responses[0].Events.AggregateRootId.ToGuid().ShouldEqual(aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => fetch_for_aggregate_written_responses[0].Events.CurrentAggregateRootVersion.ShouldEqual(0U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => fetch_for_aggregate_written_responses[0].Events.AggregateRootVersion.ShouldEqual(0U);
        It should_return_the_correct_event_source = () => fetch_for_aggregate_written_responses[0].Events.EventSourceId.ShouldEqual(event_source.Value);
        It should_return_no_events = () => fetch_for_aggregate_written_responses[0].Events.Events.ShouldBeEmpty();
    }
    
    [Tags("IntegrationTest")]
    class when_fetching_one_event_with_batching
    {
        static FetchForAggregateInBatchesRequest request;
        
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                event_to_commit.create(),
            }.ToList()), execution_context).GetAwaiter().GetResult();
            request = EventStoreRequests.FetchBatchFor(aggregate_root_id, event_source, execution_context);
        };
        
        Because of = () => event_store_service.FetchForAggregateInBatches(request, fetch_for_aggregate_response_stream, server_call_context).GetAwaiter().GetResult();

        It should_not_fail = () => fetch_for_aggregate_written_responses.ShouldEachConformTo(_ => _.Failure == default);
        It should_write_one_batch = () => fetch_for_aggregate_written_responses.Count.ShouldEqual(1);
        It should_return_the_correct_aggregate_root = () => fetch_for_aggregate_written_responses[0].Events.AggregateRootId.ToGuid().ShouldEqual(aggregate_root_id.Value);
        It should_return_the_correct_aggregate_root_version = () => fetch_for_aggregate_written_responses[0].Events.CurrentAggregateRootVersion.ShouldEqual(1U);
        It should_return_the_correct_deprecated_aggregate_root_version = () => fetch_for_aggregate_written_responses[0].Events.AggregateRootVersion.ShouldEqual(0U);
        It should_return_the_correct_event_source = () => fetch_for_aggregate_written_responses[0].Events.EventSourceId.ShouldEqual(event_source.Value);
        It should_return_one_event = () => fetch_for_aggregate_written_responses[0].Events.Events.Count.ShouldEqual(1);
    }
}