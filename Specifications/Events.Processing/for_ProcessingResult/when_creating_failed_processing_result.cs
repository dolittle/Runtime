// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_ProcessingResult
{
    public class when_creating_failed_processing_result
    {
        static FailedProcessingResult result;

        Because of = () => result = new FailedProcessingResult();

        It should_have_failed_result_value = () => result.Value.ShouldEqual(ProcessingResultValue.Failed);
    }
}