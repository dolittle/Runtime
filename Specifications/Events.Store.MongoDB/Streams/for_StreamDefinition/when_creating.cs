// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_StreamDefinition
{
    public class when_creating
    {
        static Guid stream_id;
        static AbstractFilterDefinition filter_definition;
        static bool partitioned;
        static bool @public;
        static StreamDefinition result;

        Establish context = () =>
        {
            stream_id = Guid.Parse("7e2be412-c950-48f3-bd23-66c46cba452a");
            filter_definition = new RemoteFilterDefinition();
            partitioned = true;
            @public = false;
        };

        Because of = () => result = new StreamDefinition(stream_id, filter_definition, partitioned, @public);

        It should_have_the_correct_partitioned_value = () => result.Partitioned.ShouldEqual(partitioned);
        It should_have_the_correct_public_value = () => result.Public.ShouldEqual(@public);
        It should_have_the_correct_stream_id = () => result.StreamId.ShouldEqual(stream_id);
        It should_have_the_correct_filter_definition = () => result.Filter.ShouldEqual(filter_definition);
    }
}