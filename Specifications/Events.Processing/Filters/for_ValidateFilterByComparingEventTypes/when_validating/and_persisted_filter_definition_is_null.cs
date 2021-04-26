// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_persisted_filter_definition_is_null : given.all_dependencies
    {
        static FilterValidationResult result;
        Because of = () => result = validator.Validate(null, filter_processor, CancellationToken.None).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
        It should_not_get_the_stream_processor_state = () => stream_processor_states.VerifyNoOtherCalls();
        It should_not_fetch_event_types = () => events_fetchers.VerifyNoOtherCalls();
    }
}