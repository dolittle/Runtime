// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating;

public class and_there_is_no_filter_validator_for_the_definition_type : given.all_dependencies
{
    Establish context = () =>
    {
        // type_finder
        //     .Setup(_ => _.FindMultiple(typeof(ICanValidateFilterFor<FilterDefinition>)))
        //     .Returns(Array.Empty<Type>());
    };

    static FilterValidationResult result;
    Because of = () => result = filter_validators().Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.ShouldBeFalse();
}