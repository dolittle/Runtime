// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Autofac;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating;

public class and_everything_is_fine : given.all_dependencies
{
    static FilterValidationResult validationResult;

    Establish context = () =>
    {
        validationResult = FilterValidationResult.Succeeded();

        filter_validator
            .Setup(_ => _.Validate(filter_definition, filter_processor, stream_processor_state.Position, cancellation_token))
            .Returns(Task.FromResult(validationResult));
    };

    static FilterValidationResult result;
    Because of = () => result = filter_validators_with_services(_ => _.RegisterInstance(filter_validator.Object)).Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

    It should_return_the_result_from_the_validator = () => result.ShouldBeTheSameAs(validationResult);
}