// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_ProcessingResult
{
    public class when_creating_retry_processing_result
    {
        static RetryProcessingResult result;
        static ulong timeout = 123;

        Because of = () => result = new RetryProcessingResult(timeout);

        It should_have_retry_result_value = () => result.Value.ShouldEqual(ProcessingResultValue.Retry);
        It should_have_the_same_timeout = () => result.RetryTimeout.ShouldEqual(timeout);
    }
}