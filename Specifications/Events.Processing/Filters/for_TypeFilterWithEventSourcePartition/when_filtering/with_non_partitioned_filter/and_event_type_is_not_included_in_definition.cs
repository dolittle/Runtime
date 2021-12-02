// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartition.when_filtering.with_non_partitioned_filter;

public class and_event_type_is_not_included_in_definition : given.all_dependencies
{
    static TypeFilterWithEventSourcePartition filter;
    static IFilterResult result;

    Establish context = () =>
    {
        filter = new TypeFilterWithEventSourcePartition(
            scope,
            new TypeFilterWithEventSourcePartitionDefinition(Guid.NewGuid(), Guid.NewGuid(), new ArtifactId[] { given.artifacts.single().Id }.AsEnumerable(), false),
            writer.Object,
            Moq.Mock.Of<ILogger<TypeFilterWithEventSourcePartition>>());
    };

    Because of = () => result = filter.Filter(given.committed_events.single("event source"), "a partition", Guid.NewGuid(), default).GetAwaiter().GetResult();

    It should_have_none_partition = () => result.Partition.Value.ShouldEqual(PartitionId.None.Value);
    It should_be_successful = () => result.Succeeded.ShouldBeTrue();
    It should_not_retry = () => result.Retry.ShouldBeFalse();
    It should_not_be_included = () => result.IsIncluded.ShouldBeFalse();
}