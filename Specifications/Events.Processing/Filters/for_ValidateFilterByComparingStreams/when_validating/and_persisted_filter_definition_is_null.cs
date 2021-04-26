// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_persisted_filter_definition_is_null : given.all_dependencies
    {
        static FilterValidationResult result;

        Because of = () => result = validator.Validate(null, filter_processor.Object, cancellation_token).GetAwaiter().GetResult();
        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}