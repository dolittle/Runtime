// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Machine.Specifications.Rules;
using Dolittle.Rules;
using Dolittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.Rules.for_Email
{
    public class when_checking_value_that_is_missing_at_sign
    {
        static string value = "somethingsomeplace.com";
        static Email rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new Email(null);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, value);

        It should_fail_with_invalid_email_as_reason = () => rule_context_mock.ShouldFailWith(rule, value, Email.InvalidEMailReason);
    }
}
