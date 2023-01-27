// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.Filters.for_TypeFilterWithEventSourcePartitionDefinition;

public class when_creating
{
    static StreamId source_stream;
    static StreamId target_stream;
    static IEnumerable<ArtifactId> types;
    static bool partitioned;
    static TypeFilterWithEventSourcePartitionDefinition definition;

    Establish context = () =>
    {
        source_stream = Guid.NewGuid();
        target_stream = Guid.NewGuid();
        types = new ArtifactId[] { given.artifacts.single().Id }.AsEnumerable();
        partitioned = true;
    };

    Because of = () => definition = new TypeFilterWithEventSourcePartitionDefinition(source_stream, target_stream, types, partitioned);

    It should_have_the_correct_source_stream = () => definition.SourceStream.Should().Be(source_stream);
    It should_have_the_correct_target_stream = () => definition.TargetStream.Should().Be(target_stream);
    It should_have_the_correct_types = () => definition.Types.Should().Contain(types);
    It should_have_the_correct_partitioned_value = () => definition.Partitioned.Should().Be(partitioned);
}