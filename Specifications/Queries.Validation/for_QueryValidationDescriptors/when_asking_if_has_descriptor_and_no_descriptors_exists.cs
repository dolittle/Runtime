// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptors
{
    public class when_asking_if_has_descriptor_and_no_descriptors_exists : given.no_query_validation_descriptors
    {
        static bool result;

        Because of = () => result = descriptors.HasDescriptorFor<SimpleQuery>();

        It should_not_have_any = () => result.ShouldBeFalse();
    }
}
