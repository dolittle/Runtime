// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating
{
    public class and_it_can_validate_definition : given.all_dependencies
    {
        static Moq.Mock<IFilterProcessor<IFilterDefinition>> filter_processor;
        static Moq.Mock<ICanValidateFilterFor<IFilterDefinition>> validator;

        Establish context = () =>
        {
            filter_processor = new Moq.Mock<IFilterProcessor<IFilterDefinition>>();
            validator = new Moq.Mock<ICanValidateFilterFor<IFilterDefinition>>();
            container.Setup(_ => _.Get<ICanValidateFilterFor<IFilterDefinition>>()).Returns(validator.Object);
        };

        Because of = () => filter_validators.Validate(filter_processor.Object);

        It should_validate_the_filter = () => validator.Verify(_ => _.Validate(filter_processor.Object, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}