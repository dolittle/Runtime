// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating;

public class and_getting_the_persisted_filter_definition_fails : given.all_dependencies
{
    Establish context = () =>
    {
        filter_definitions
            .Setup(_ => _.TryGetFromStream(scope_id, filter_target_stream, cancellation_token))
            .Returns(Task.FromResult(Try<IFilterDefinition>.Failed(new Exception())));
    };

    static FilterValidationResult result;
    Because of = () => result = filter_validators().Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();
    It should_fail_validation = () => result.Success.ShouldBeFalse();
}