// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Validation;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidator
{
    public class when_validating_query_with_descriptor : given.a_query_validator
    {
        static SomeQuery query;
        static Mock<IQueryValidationDescriptor> descriptor_mock;
        static QueryValidationResult result;

        Establish context = () =>
        {
            query = new SomeQuery();
            query_validation_descriptors_mock.Setup(q => q.HasDescriptorFor<SomeQuery>()).Returns(true);
            descriptor_mock = new Mock<IQueryValidationDescriptor>();
            descriptor_mock.SetupGet(d => d.ArgumentRules).Returns(Array.Empty<IValueRule>());
            query_validation_descriptors_mock.Setup(q => q.GetDescriptorFor<SomeQuery>()).Returns(descriptor_mock.Object);
        };

        Because of = () => result = query_validator.Validate(query);

        It should_return_a_valid_result = () => result.Success.ShouldBeTrue();
        It should_check_if_has_descriptors = () => query_validation_descriptors_mock.Verify(q => q.HasDescriptorFor<SomeQuery>(), Times.Once());
        It should_get_descriptor = () => query_validation_descriptors_mock.Verify(q => q.GetDescriptorFor<SomeQuery>(), Times.Once());
    }
}
