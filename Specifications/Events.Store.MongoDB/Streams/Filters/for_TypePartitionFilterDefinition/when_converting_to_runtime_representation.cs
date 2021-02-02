// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters.for_TypePartitionFilterDefinition
{
    public class when_converting_to_runtime_representation
    {
        static Guid stream_id;
        static bool partitioned;
        static IEnumerable<Guid> types;
        static TypePartitionFilterDefinition filter_definition;
        static IFilterDefinition result;

        Establish context = () =>
        {
            stream_id = Guid.Parse("8b31476b-fde6-4044-9129-b5cc93987529");
            partitioned = true;
            types = new[]
            {
                Guid.Parse("f0d8f3b0-9e59-4879-93da-d1e230d88493"),
                Guid.Parse("19192073-b0ba-46a9-a1cd-f75bfc9bcd20"),
                Guid.Parse("a423f803-3c73-49eb-a517-78b0311b1b00")
            };
            filter_definition = new TypePartitionFilterDefinition(types);
        };

        Because of = () => result = filter_definition.AsRuntimeRepresentation(stream_id, partitioned, true);

        It should_be_a_type_partition_filter_definition = () => result.ShouldBeOfExactType<TypeFilterWithEventSourcePartitionDefinition>();
        It should_be_partitioned = () => result.Partitioned.ShouldBeTrue();
        It should_not_be_public = () => result.Public.ShouldBeFalse();
        It should_have_the_event_log_as_source_stream = () => result.SourceStream.ShouldEqual(StreamId.EventLog);
        It should_have_the_correct_target_stream = () => result.TargetStream.Value.ShouldEqual(stream_id);
        It should_have_the_correct_types = () => (result as TypeFilterWithEventSourcePartitionDefinition).Types.ShouldContainOnly(types.Select(_ => new ArtifactId(_)));
    }
}