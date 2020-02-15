// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_ProcessingResult
{
    public class when_retry_processing
    {
        const string reason = "some reason";
        static RetryProcessingResult result;
        static uint timeout = 123;

        Because of = () => result = new RetryProcessingResult(timeout, reason);

        It should_not_be_succeeded = () => result.Succeeded.ShouldEqual(false);
        It should_retry = () => result.Retry.ShouldEqual(true);
        It should_have_the_correct_timeout = () => result.RetryTimeout.ShouldEqual(timeout);
        It should_have_the_correct_reason = () => result.FailureReason.ShouldEqual(reason);
    }
}