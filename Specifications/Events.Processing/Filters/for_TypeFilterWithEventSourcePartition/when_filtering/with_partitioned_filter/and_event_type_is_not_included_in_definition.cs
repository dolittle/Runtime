// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartition.when_filtering.with_partitioned_filter
{
    public class and_event_type_is_not_included_in_definition : given.all_dependencies
    {
        static PartitionId partition;
        static TypeFilterWithEventSourcePartition filter;
        static IFilterResult result;

        Establish context = () =>
        {
            partition = Guid.NewGuid();
            filter = new TypeFilterWithEventSourcePartition(
                scope,
                new TypeFilterWithEventSourcePartitionDefinition(Guid.NewGuid(), Guid.NewGuid(), new ArtifactId[] { given.artifacts.single().Id }.AsEnumerable(), true),
                writer.Object,
                Moq.Mock.Of<ILogger>());
        };

        Because of = () => result = filter.Filter(given.committed_events.single(partition.Value), Guid.NewGuid(), Guid.NewGuid(), default).GetAwaiter().GetResult();

        It should_have_the_correct_partition = () => result.Partition.ShouldEqual(partition);
        It should_be_successful = () => result.Succeeded.ShouldBeTrue();
        It should_not_retry = () => result.Retry.ShouldBeFalse();
        It should_not_be_included = () => result.IsIncluded.ShouldBeFalse();
    }
}