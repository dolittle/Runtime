// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Rules;
using Dolittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.Rules.for_Required
{
    public class when_evaluating_empty_string
    {
        static Required rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new Required(null);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, string.Empty);

        It should_fail_with_string_is_empty_as_reason = () => rule_context_mock.Verify(r => r.Fail(rule, string.Empty, Moq.It.Is<Cause>(_ => _.Reason == Required.StringIsEmpty)), Moq.Times.Once());
    }
}
