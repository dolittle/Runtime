// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.fast_event_handler.needing_to_catchup.processing_events;

class without_problems : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;

    Establish context = () =>
    {
        commit_events_for_each_event_type(10, "some event source").GetAwaiter().GetResult();
        complete_after_processing_number_of_events(committed_events.Count);
        with_event_handlers((true, number_of_event_types, ScopeId.Default, true));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        // stop_event_handlers_after(TimeSpan.FromSeconds(10));
        run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers().GetAwaiter().GetResult();
    };

    It should_have_persisted_stream_definition = () => get_stream_definition_for(event_handler).Success.ShouldBeTrue();
    It should_have_persisted_a_partitioned_stream_definition = () => get_stream_definition_for(event_handler).Result.Partitioned.ShouldBeTrue();
    It should_have_persisted_a_non_public_stream_definition = () => get_stream_definition_for(event_handler).Result.Public.ShouldBeFalse();
    It should_have_persisted_stream_definition_with_correct_filter_definition_type = () => get_stream_definition_for(event_handler).Result.FilterDefinition.ShouldBeOfExactType<TypeFilterWithEventSourcePartitionDefinition>();
    It should_have_persisted_stream_definition_with_partitioned_filter_definition = () => get_filter_definition_for<TypeFilterWithEventSourcePartitionDefinition>(event_handler).Partitioned.ShouldBeTrue();
    It should_have_persisted_stream_definition_with_non_public_filter_definition = () => get_filter_definition_for<TypeFilterWithEventSourcePartitionDefinition>(event_handler).Public.ShouldBeFalse();
    It should_have_persisted_stream_definition_with_filter_definition_with_event_log_as_source_stream = () => get_filter_definition_for<TypeFilterWithEventSourcePartitionDefinition>(event_handler).SourceStream.ShouldEqual(StreamId.EventLog);
    It should_have_persisted_stream_definition_with_filter_definition_with_correct_target_stream = () => get_filter_definition_for<TypeFilterWithEventSourcePartitionDefinition>(event_handler).SourceStream.ShouldEqual(StreamId.EventLog);
}