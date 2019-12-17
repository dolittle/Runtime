// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptorFor
{
    public class when_describing_same_argument_twice : given.an_empty_query_validation_descriptor
    {
        Because of = () =>
        {
            descriptor.ForArgument(q => q.IntegerArgument);
            descriptor.ForArgument(q => q.IntegerArgument);
        };

        It should_only_have_one_builder = () => descriptor.ArgumentsRuleBuilders.Count().ShouldEqual(1);
    }
}
