// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamDefinition;

public class from_public_filter_definition
{
    static StreamId source_stream_id;
    static StreamId target_stream_id;
    static PublicFilterDefinition filter_definition;
    static StreamDefinition stream_definition;

    Establish context = () =>
    {
        source_stream_id = Guid.Parse("42890d11-9060-4c1a-91c7-0f81374427c5");
        target_stream_id = Guid.Parse("76098767-5fde-4296-9da6-80624fda8592");
        filter_definition = new PublicFilterDefinition(source_stream_id, target_stream_id);
    };

    Because of = () => stream_definition = new StreamDefinition(filter_definition);

    It should_be_partitioned = () => stream_definition.Partitioned.ShouldBeTrue();
    It should_be_public = () => stream_definition.Public.ShouldBeTrue();
    It should_have_the_correct_stream_id = () => stream_definition.StreamId.ShouldEqual(target_stream_id);
    It should_have_the_correct_filter_definition = () => stream_definition.FilterDefinition.ShouldEqual(filter_definition);
}