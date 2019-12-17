// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptorFor
{
    public class when_describing_argument : given.an_empty_query_validation_descriptor
    {
        static QueryArgumentValidationBuilder<SomeQuery, int> builder;

        Because of = () => builder = descriptor.ForArgument(q => q.IntegerArgument);

        It should_hold_the_builder = () => descriptor.ArgumentsRuleBuilders.ShouldContainOnly(builder);
        It should_return_a_builder = () => builder.ShouldNotBeNull();
    }
}
