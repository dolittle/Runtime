// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_store_stream_event
{
    public class an_aggregate_event
    {
        static CommittedAggregateEvent committed_event;
        static IEventConverter event_converter;
        static StreamPosition stream_position;
        static PartitionId partition;
        static StreamEvent result;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_aggregate_event(0, Guid.NewGuid(), Guid.NewGuid(), 1);
            event_converter = new EventConverter();
            stream_position = 3;
            partition = Guid.NewGuid();
        };

        Because of = () => result = event_converter.ToStoreStreamEvent(committed_event, stream_position, partition);

        It should_have_the_same_content = () => result.Content.ToString().ShouldEqual(committed_event.Content);
        It should_represent_the_same_event = () => result.ShouldRepresentTheSameBaseEventAs(committed_event, stream_position, partition);
        It should_be_applied_by_aggregate = () => result.Aggregate.WasAppliedByAggregate.ShouldBeTrue();
        It should_have_the_same_aggregate_root_type_generation = () => result.Aggregate.TypeGeneration.ShouldEqual(committed_event.AggregateRoot.Generation.Value);
        It should_have_the_same_aggregate_root_type_id = () => result.Aggregate.TypeId.ShouldEqual(committed_event.AggregateRoot.Id.Value);
        It should_have_the_same_aggregate_root_version = () => result.Aggregate.Version.ShouldEqual(committed_event.AggregateRootVersion.Value);
        It should_not_come_from_event_horizon = () => result.EventHorizonMetadata.FromEventHorizon.ShouldBeFalse();
    }
}