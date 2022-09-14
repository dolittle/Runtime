// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.when_fetching_aggregate_events;

class specs : given.a_clean_event_store
{
    static ArtifactId aggregate_root_id;
    static EventSourceId event_source;
    static IList<FetchForAggregateResponse> response;
    
    Establish context = () =>
    {
        aggregate_root_id = "7348f567-d45f-4d9b-9b8e-32eff7282ea0";
        event_source = "some event source";
    };
    
    [Tags("IntegrationTest")]
    class for_a_tenant_that_is_not_configured
    {
        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, execution_context with {Tenant = "d48ca32c-bc98-4d6e-9e8d-4eaaf5adb579"}).ToListAsync().GetAwaiter().GetResult();

        It should_return_one_message = () => response.Count.ShouldEqual(1);
        It should_fail = () => response[0].Failure.ShouldNotBeNull();
    }
    
    [Tags("IntegrationTest")]
    class and_there_are_no_events_for_aggregate
    {
        Establish context = () =>
        {
            event_store.Commit(new UncommittedAggregateEvents("another event source", new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                given.event_to_commit.create()
            }.ToList()), execution_context).GetAwaiter().GetResult();
        };
        
        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, execution_context).ToListAsync().GetAwaiter().GetResult();

        It should_return_one_message = () => response.Count.ShouldEqual(1);
        It should_not_fail = () => response[0].Failure.ShouldBeNull();
        It should_have_no_aggregate_events = () => response[0].Events.Events.ShouldBeEmpty();
    }
    
    [Tags("IntegrationTest")]
    class and_there_is_one_event_for_aggregate
    {
        static UncommittedAggregateEvents uncommitted_events;

        Establish context = () =>
        {
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                new Artifact(aggregate_root_id, ArtifactGeneration.First),
                AggregateRootVersion.Initial,
                new []{given.event_to_commit.create()});
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, execution_context).ToListAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.ShouldNotContain(_ => _.Failure != default);

        It should_return_the_correct_committed_event = () => response.should_be_the_correct_response(uncommitted_events, execution_context, EventLogSequenceNumber.Initial, uncommitted_events.ExpectedAggregateRootVersion);
    }
    
    [Tags("IntegrationTest")]
    class and_there_are_multiple_events_for_aggregate
    {
        static UncommittedAggregateEvents uncommitted_events;

        Establish context = () =>
        {
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                new Artifact(aggregate_root_id, ArtifactGeneration.First),
                AggregateRootVersion.Initial,
                new []{given.event_to_commit.create(), given.event_to_commit.create(), given.event_to_commit.create()});
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, execution_context).ToListAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.ShouldNotContain(_ => _.Failure != default);

        It should_return_the_correct_committed_event = () => response.should_be_the_correct_response(uncommitted_events, execution_context, EventLogSequenceNumber.Initial, uncommitted_events.ExpectedAggregateRootVersion);
    }
}