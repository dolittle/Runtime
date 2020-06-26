// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters.for_RemoteFilterDefinition.when_converting_to_runtime_representation
{
    public class and_it_is_a_public_filter
    {
        static Guid stream_id;
        static bool partitioned;
        static RemoteFilterDefinition filter_definition;
        static IFilterDefinition result;

        Establish context = () =>
        {
            stream_id = Guid.Parse("8b31476b-fde6-4044-9129-b5cc93987529");
            partitioned = true;
            filter_definition = new RemoteFilterDefinition();
        };

        Because of = () => result = filter_definition.AsRuntimeRepresentation(stream_id, partitioned, true);

        It should_be_a_public_filter_definition = () => result.ShouldBeOfExactType<PublicFilterDefinition>();
        It should_be_partitioned = () => result.Partitioned.ShouldBeTrue();
        It should_be_public = () => result.Public.ShouldBeTrue();
        It should_have_the_event_log_as_source_stream = () => result.SourceStream.ShouldEqual(StreamId.EventLog);
        It should_have_the_correct_target_stream = () => result.TargetStream.Value.ShouldEqual(stream_id);
    }
}