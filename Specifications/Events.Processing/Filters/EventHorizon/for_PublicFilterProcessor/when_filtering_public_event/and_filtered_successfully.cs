// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon.for_PublicFilterProcessor.when_filtering_public_event;

public class and_filtered_successfully : given.a_public_event_filter
{
    static IFilterResult result;
    static PartitionedFilterResponse filter_response;
    static bool is_included;
    static PartitionId partition_id;

    Establish context = () =>
    {
        is_included = true;
        partition_id = "partition";
        filter_response = new PartitionedFilterResponse { IsIncluded = is_included, PartitionId = partition_id.Value };
        dispatcher.Setup(_ => _.Call(Moq.It.IsAny<FilterEventRequest>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(filter_response));
    };

    Because of = () => result = filter.Filter(a_public_event, "a partition", Guid.NewGuid(), default).GetAwaiter().GetResult();

    It should_call_the_remote_filter = () => dispatcher.Verify(_ => _.Call(Moq.It.IsAny<FilterEventRequest>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_filter_successfully = () => result.Succeeded.ShouldBeTrue();
    It should_not_have_failure_reason = () => result.FailureReason.ShouldBeEmpty();
    It should_not_retry = () => result.Retry.ShouldBeFalse();
    It should_be_included = () => result.IsIncluded.ShouldEqual(is_included);
}