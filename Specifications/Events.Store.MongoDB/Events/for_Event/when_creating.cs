// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_Event;

public class when_creating
{
    static ulong event_log_sequence_number;
    static ExecutionContext execution_context;
    static EventMetadata event_metadata;
    static AggregateMetadata aggregate_metadata;
    static EventHorizonMetadata event_horizon_metadata;
    static BsonDocument content;
    static Event result;

    Establish context = () =>
    {
        event_log_sequence_number = random.event_log_sequence_number;
        execution_context = execution_contexts.create_store();
        event_metadata = metadata.random_event_metadata;
        aggregate_metadata = metadata.aggregate_metadata_from_non_aggregate_event;
        event_horizon_metadata = metadata.random_event_horizon_metadata;
        content = BsonDocument.Parse("{\"something\": \"something\"}");
    };

    Because of = () => result = new Event(
        event_log_sequence_number,
        execution_context,
        event_metadata,
        aggregate_metadata,
        event_horizon_metadata,
        content);

    It should_have_the_correct_event_log_sequence_number = () => result.EventLogSequenceNumber.Should().Be(event_log_sequence_number);
    It should_have_the_correct_execution_context = () => result.ExecutionContext.Should().Be(execution_context);
    It should_have_the_correct_event_metadata = () => result.Metadata.Should().Be(event_metadata);
    It should_have_the_correct_aggregate_metadata = () => result.Aggregate.Should().Be(aggregate_metadata);
    It should_have_the_correct_event_horizon_metadata = () => result.EventHorizon.Should().Be(event_horizon_metadata);
    It should_have_the_correct_content = () => result.Content.Should().BeEquivalentTo(content);
}