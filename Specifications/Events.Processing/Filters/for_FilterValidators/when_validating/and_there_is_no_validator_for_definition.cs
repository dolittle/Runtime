// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating
{
    public class and_there_is_no_validator_for_definition : given.all_dependencies
    {
        static Exception exception;

        Establish context = () => container.Setup(_ => _.Get<ICanValidateFilterFor<IFilterDefinition>>()).Returns<ICanValidateFilterFor<IFilterDefinition>>(null);

        Because of = () => exception = Catch.Exception(() => filter_validators.Validate(Moq.Mock.Of<IFilterProcessor<IFilterDefinition>>()));

        It should_fail_because_there_are_no_validator_for_definition = () => exception.ShouldBeOfExactType<CannotValidateFilterWithDefinitionType>();
    }
}