// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Validation;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.for_ValueValidationBuilder
{
    public class when_adding_rule
    {
        static ValueValidationBuilder<object> builder;
        static Mock<IValueRule> rule_mock;

        Establish context = () =>
        {
            builder = new ValueValidationBuilder<object>(null);
            rule_mock = new Mock<IValueRule>();
        };

        Because of = () => builder.AddRule(rule_mock.Object);

        It should_hold_the_rule = () => builder.Rules.ShouldContainOnly(rule_mock.Object);
    }
}
