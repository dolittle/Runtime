// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using CommittedAggregateEvents = Dolittle.Runtime.Events.Store.CommittedAggregateEvents;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;

namespace Integration.Tests.Events.Store.when_fetching_aggregate_events.in_batches;

class specs : given.a_clean_event_store
{
    static ArtifactId aggregate_root_id;
    static EventSourceId event_source;
    static List<ArtifactId> event_types;
    static FetchForAggregateResponse[] response;
    
    
    Establish context = () =>
    {
        aggregate_root_id = "7348f567-d45f-4d9b-9b8e-32eff7282ea0";
        event_source = "some event source";
        event_types = new List<ArtifactId>();
    };
    
    [Tags("IntegrationTest")]
    class for_a_tenant_that_is_not_configured
    {
        Establish context = () =>
        {
            event_types.Add("270b39d9-cfe3-455c-968a-c0b01002e84b");
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context with {Tenant = "d48ca32c-bc98-4d6e-9e8d-4eaaf5adb579"}).ToArrayAsync().GetAwaiter().GetResult();

        It should_fail = () => response.All(_ => _.Failure is not null).ShouldBeTrue();
    }
    
    [Tags("IntegrationTest")]
    class and_filtering_on_no_event_types
    {
        static UncommittedAggregateEvents uncommitted_events;
        Establish context = () =>
        {
            uncommitted_events = new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                given.event_to_commit.create(),
                given.event_to_commit.create(),
                given.event_to_commit.create()
            }.ToList());
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };
        
        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();

        It should_return_no_committed_events = () => response.Combine().Events.Events.ShouldBeEmpty();
    }
    
    [Tags("IntegrationTest")]
    class and_there_are_no_events_for_aggregate
    {
        static UncommittedAggregateEvents uncommitted_events;
        Establish context = () =>
        {
            event_types.Add("270b39d9-cfe3-455c-968a-c0b01002e84b");
            uncommitted_events = new UncommittedAggregateEvents("another event source", new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                given.event_to_commit.create()
            }.ToList());
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };
        
        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();
        It should_return_no_committed_events = () => response.Combine().Events.Events.ShouldBeEmpty();
    }
    
    [Tags("IntegrationTest")]
    class and_there_are_no_events_for_aggregate_when_getting_all
    {
        static UncommittedAggregateEvents uncommitted_events;
        Establish context = () =>
        {
            event_types.Add("270b39d9-cfe3-455c-968a-c0b01002e84b");
            uncommitted_events = new UncommittedAggregateEvents("another event source", new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                given.event_to_commit.create()
            }.ToList());
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };
        
        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();
        It should_return_no_committed_events = () => response.Combine().Events.Events.ShouldBeEmpty();
    }

    
    [Tags("IntegrationTest")]
    class and_there_is_one_event_for_aggregate_with_unwanted_event_type
    {
        static UncommittedAggregateEvents uncommitted_events;
        Establish context = () =>
        {
            event_types.Add("270b39d9-cfe3-455c-968a-c0b01002e84b");
            uncommitted_events = new UncommittedAggregateEvents(event_source, new Artifact(aggregate_root_id, ArtifactGeneration.First), AggregateRootVersion.Initial, new[]
            {
                given.event_to_commit.create()
            }.ToList());
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };
        
        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();
        It should_return_no_committed_events = () => response.Combine().Events.Events.ShouldBeEmpty();
    }
    
    [Tags("IntegrationTest")]
    class and_there_is_one_event_for_aggregate_with_wanted_event_type
    {
        static UncommittedAggregateEvents uncommitted_events;
        static ArtifactId event_type;

        Establish context = () =>
        {
            event_type = "36e5c743-d647-491a-ae79-7fb2af1cc31b";
            event_types.Add(event_type);
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                new Artifact(aggregate_root_id, ArtifactGeneration.First),
                AggregateRootVersion.Initial,
                new []{given.event_to_commit.create_with_type(event_type)});
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();

        It should_return_the_correct_committed_event = () => should_extensions.events_should_be_the_same(
            response.Combine().Events.ToCommittedEvents(),
            uncommitted_events,
            execution_context,
            EventLogSequenceNumber.Initial,
            uncommitted_events.ExpectedAggregateRootVersion);
    }

    [Tags("IntegrationTest")]
    class and_there_are_multiple_events_for_aggregate_with_wanted_event_type
    {
        static UncommittedAggregateEvents uncommitted_events;
        static ArtifactId event_type;

        Establish context = () =>
        {
            event_type = "36e5c743-d647-491a-ae79-7fb2af1cc31b";
            event_types.Add(event_type);
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                new Artifact(aggregate_root_id, ArtifactGeneration.First),
                AggregateRootVersion.Initial,
                new []{given.event_to_commit.create_with_type(event_type), given.event_to_commit.create_with_type(event_type), given.event_to_commit.create_with_type(event_type)});
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();

        It should_return_the_correct_committed_event = () => should_extensions.events_should_be_the_same(
            response.Combine().Events.ToCommittedEvents(),
            uncommitted_events,
            execution_context,
            EventLogSequenceNumber.Initial,
            uncommitted_events.ExpectedAggregateRootVersion);
    }
    
    [Tags("IntegrationTest")]
    class and_there_are_multiple_events_for_aggregate_with_a_mix_of_wanted_and_unwanted_event_types
    {
        static UncommittedAggregateEvents uncommitted_events;
        static ArtifactId event_type;

        Establish context = () =>
        {
            event_type = "36e5c743-d647-491a-ae79-7fb2af1cc31b";
            event_types.Add(event_type);
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                new Artifact(aggregate_root_id, ArtifactGeneration.First),
                AggregateRootVersion.Initial,
                new []
                {
                    given.event_to_commit.create(),
                    given.event_to_commit.create_with_type(event_type),
                    given.event_to_commit.create(),
                    given.event_to_commit.create_with_type(event_type),
                    given.event_to_commit.create(),
                    given.event_to_commit.create_with_type(event_type)
                });
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, event_types, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();

        It should_return_the_correct_committed_event = () => should_extensions.events_should_be_the_same(
            response.Combine().Events.ToCommittedEvents(),
            new UncommittedAggregateEvents(
                uncommitted_events.EventSource,
                uncommitted_events.AggregateRoot,
                uncommitted_events.ExpectedAggregateRootVersion,
                uncommitted_events.Where(_ => _.Type.Id == event_type).ToList()),
            execution_context,
            null,
            null);
    }
    
    [Tags("IntegrationTest")]
    class and_getting_all_aggregate_events
    {
        static UncommittedAggregateEvents uncommitted_events;
        static ArtifactId event_type;

        Establish context = () =>
        {
            event_type = "36e5c743-d647-491a-ae79-7fb2af1cc31b";
            event_types.Add(event_type);
            uncommitted_events = new UncommittedAggregateEvents(
                event_source,
                new Artifact(aggregate_root_id, ArtifactGeneration.First),
                AggregateRootVersion.Initial,
                new []
                {
                    given.event_to_commit.create(),
                    given.event_to_commit.create_with_type(event_type),
                    given.event_to_commit.create(),
                    given.event_to_commit.create_with_type(event_type),
                    given.event_to_commit.create(),
                    given.event_to_commit.create_with_type(event_type)
                });
            event_store.Commit(uncommitted_events, execution_context).GetAwaiter().GetResult();
        };

        Because of = () => response = event_store.FetchForAggregate(aggregate_root_id, event_source, execution_context).ToArrayAsync().GetAwaiter().GetResult();

        It should_not_fail = () => response.All(_ => _.Failure is null).ShouldBeTrue();

        It should_return_the_correct_committed_event = () => should_extensions.events_should_be_the_same(
            response.Combine().Events.ToCommittedEvents(),
            uncommitted_events,
            execution_context,
            null,
            null);
    }
}