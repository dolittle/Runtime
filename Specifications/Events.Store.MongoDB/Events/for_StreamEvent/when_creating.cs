// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_StreamEvent;

public class when_creating
{
    static ulong stream_position;
    static string partition;
    static ExecutionContext execution_context;
    static StreamEventMetadata stream_event_metadata;
    static AggregateMetadata aggregate_metadata;
    static EventHorizonMetadata event_horizon_metadata;
    static BsonDocument content;
    static StreamEvent result;

    Establish context = () =>
    {
        stream_position = random.stream_position;
        partition = "   partition   ˝";
        execution_context = execution_contexts.create_store();
        stream_event_metadata = metadata.random_stream_event_metadata;
        aggregate_metadata = metadata.aggregate_metadata_from_non_aggregate_event;
        event_horizon_metadata = metadata.random_event_horizon_metadata;
        content = BsonDocument.Parse("{\"something\": \"something\"}");
    };

    Because of = () => result = new StreamEvent(
        stream_position,
        partition,
        execution_context,
        stream_event_metadata,
        aggregate_metadata,
        event_horizon_metadata,
        content);

    It should_have_the_correct_stream_position = () => result.StreamPosition.Should().Be(stream_position);
    It should_have_the_correct_partition = () => result.Partition.Should().Be(partition);
    It should_have_the_correct_execution_context = () => result.ExecutionContext.Should().Be(execution_context);
    It should_have_the_correct_stream_event_metadata = () => result.Metadata.Should().Be(stream_event_metadata);
    It should_have_the_correct_aggregate_metadata = () => result.Aggregate.Should().Be(aggregate_metadata);
    It should_have_the_correct_event_horizon_metadata = () => result.EventHorizon.Should().Be(event_horizon_metadata);
    It should_have_the_correct_content = () => result.Content.Should().BeEquivalentTo(content);
}