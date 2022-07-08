// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamDefinition;

public class from_non_public_filter_definition
{
    static StreamId source_stream_id;
    static StreamId target_stream_id;
    static bool partitioned;
    static FilterDefinition filter_definition;
    static StreamDefinition stream_definition;

    Establish context = () =>
    {
        source_stream_id = Guid.Parse("321c62e2-808d-49b4-9b26-b8204a4fc437");
        target_stream_id = Guid.Parse("7c5b1f31-e348-4acf-befd-22e43f354c9b");
        partitioned = false;
        filter_definition = new FilterDefinition(source_stream_id, target_stream_id, partitioned);
    };

    Because of = () => stream_definition = new StreamDefinition(filter_definition);

    It should_have_there_correct_partitioned_value = () => stream_definition.Partitioned.ShouldEqual(partitioned);
    It should_not_be_public = () => stream_definition.Public.ShouldBeFalse();
    It should_have_the_correct_stream_id = () => stream_definition.StreamId.ShouldEqual(target_stream_id);
    It should_have_the_correct_filter_definition = () => stream_definition.FilterDefinition.ShouldEqual(filter_definition);
}