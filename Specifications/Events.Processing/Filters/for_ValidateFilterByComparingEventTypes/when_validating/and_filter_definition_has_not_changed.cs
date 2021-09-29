// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_filter_definition_has_not_changed : given.all_dependencies
    {
        static FilterValidationResult result;
        Because of = () => result = validator.Validate(filter_definition, filter_processor, 43, cancellation_token).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Success.ShouldBeTrue();
        It should_not_fetch_event_types = () => events_fetchers.VerifyNoOtherCalls();
    }
}