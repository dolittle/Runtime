// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartition.when_filtering.with_non_partitioned_filter;

public class and_event_type_is_included_in_definition : given.all_dependencies
{
    static Artifact artifact;
    static TypeFilterWithEventSourcePartition filter;
    static IFilterResult result;

    Establish context = () =>
    {
        artifact = given.artifacts.single();
        filter = new TypeFilterWithEventSourcePartition(
            scope,
            new TypeFilterWithEventSourcePartitionDefinition(Guid.NewGuid(), Guid.NewGuid(), new ArtifactId[] { artifact.Id }.AsEnumerable(), false),
            writer.Object,
            Moq.Mock.Of<ILogger<TypeFilterWithEventSourcePartition>>());
    };

    Because of = () => result = filter.Filter(given.committed_events.single_with_artifact("event source", artifact), "a partition", Guid.NewGuid(), execution_contexts.create(), default).GetAwaiter().GetResult();

    It should_have_none_partition = () => result.Partition.Value.Should().Be(PartitionId.None.Value);
    It should_be_successful = () => result.Succeeded.Should().BeTrue();
    It should_not_retry = () => result.Retry.Should().BeFalse();
    It should_be_included = () => result.IsIncluded.Should().BeTrue();
}