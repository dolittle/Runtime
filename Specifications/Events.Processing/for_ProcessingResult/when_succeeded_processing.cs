// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_ProcessingResult
{
    public class when_succeeded_processing
    {
        static SucceededProcessingResult result;

        Because of = () => result = new SucceededProcessingResult();

        It should_be_succeeded = () => result.Succeeded.ShouldEqual(true);
        It should_not_retry = () => result.Retry.ShouldEqual(false);
    }
}