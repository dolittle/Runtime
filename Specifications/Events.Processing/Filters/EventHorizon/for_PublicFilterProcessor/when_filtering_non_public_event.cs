// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Contracts;
using FluentAssertions;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon.for_PublicFilterProcessor;

public class when_filtering_non_public_event : given.a_public_event_filter
{
    static IFilterResult result;

    Because of = () => result = filter.Filter(a_non_public_event, "the partition id", Guid.NewGuid(), a_non_public_event.ExecutionContext, default).GetAwaiter().GetResult();

    It should_not_call_the_remote_filter = () => dispatcher.Verify(_ => _.Call(Moq.It.IsAny<FilterEventRequest>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
    It should_filter_successfully = () => result.Succeeded.Should().BeTrue();
    It should_not_have_failure_reason = () => result.FailureReason.Should().BeEmpty();
    It should_not_retry = () => result.Retry.Should().BeFalse();
    It should_not_be_included = () => result.IsIncluded.Should().BeFalse();
}