// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterDefinition
{
    public class when_creating
    {
        static StreamId source_stream;
        static StreamId target_stream;
        static bool partitioned;
        static FilterDefinition definition;

        Establish context = () =>
        {
            source_stream = Guid.NewGuid();
            target_stream = Guid.NewGuid();
            partitioned = true;
        };

        Because of = () => definition = new FilterDefinition(source_stream, target_stream, partitioned);

        It should_have_the_correct_source_stream = () => definition.SourceStream.ShouldEqual(source_stream);
        It should_have_the_correct_target_stream = () => definition.TargetStream.ShouldEqual(target_stream);
        It should_have_the_correct_partitioned_value = () => definition.Partitioned.ShouldEqual(partitioned);
    }
}