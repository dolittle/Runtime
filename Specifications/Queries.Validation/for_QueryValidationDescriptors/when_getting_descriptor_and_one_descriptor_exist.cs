// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptors
{
    public class when_getting_descriptor_and_one_descriptor_exist : given.one_query_validation_descriptor_for_query
    {
        static IQueryValidationDescriptor result;

        Because of = () => result = descriptors.GetDescriptorFor<SimpleQuery>();

        It should_return_the_descriptor = () => result.ShouldEqual(descriptor);
    }
}
