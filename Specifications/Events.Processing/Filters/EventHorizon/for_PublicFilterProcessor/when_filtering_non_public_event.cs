// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon.for_PublicFilterProcessor
{
    public class when_filtering_non_public_event : given.a_public_event_filter
    {
        static IFilterResult result;

        Because of = () => result = filter.Filter(a_non_public_event, "the partition id", Guid.NewGuid(), default).GetAwaiter().GetResult();

        It should_not_call_the_remote_filter = () => dispatcher.Verify(_ => _.Call(Moq.It.IsAny<FilterEventRequest>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
        It should_filter_successfully = () => result.Succeeded.ShouldBeTrue();
        It should_not_have_failure_reason = () => result.FailureReason.ShouldBeEmpty();
        It should_not_retry = () => result.Retry.ShouldBeFalse();
        It should_not_be_included = () => result.IsIncluded.ShouldBeFalse();
    }
}