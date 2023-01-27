// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating;

public class and_filter_definitions_are_not_equal : given.all_dependencies
{
    Establish context = () =>
    {
        definition_comparer
            .Setup(_ => _.DefinitionsAreEqual(filter_definition, filter_definition))
            .Returns(FilterValidationResult.Failed("something went wrong"));
    };

    static FilterValidationResult result;
    Because of = () => result = filter_validators_with_services(_ => _.RegisterInstance(filter_validator.Object)).Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.Should().BeFalse();
}