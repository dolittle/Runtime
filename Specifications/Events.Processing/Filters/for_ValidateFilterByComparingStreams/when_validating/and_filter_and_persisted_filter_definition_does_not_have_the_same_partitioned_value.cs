// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_filter_and_persisted_filter_definition_does_not_have_the_same_partitioned_value : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
        };

        Because of = () => result = validator.Validate(new FilterDefinition(source_stream, target_stream, false), filter_processor.Object, cancellation_token).GetAwaiter().GetResult();
        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
    }
}