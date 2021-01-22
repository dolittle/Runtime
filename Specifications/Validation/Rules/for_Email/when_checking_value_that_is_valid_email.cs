// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rules;
using Dolittle.Runtime.Rules.Machine.Specifications;
using Dolittle.Runtime.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Validation.Rules.for_Email
{
    public class when_checking_value_that_is_valid_email
    {
        static string value = "something@someplace.com";
        static Email rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new Email(null);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, value);

        It should_not_fail = () => rule_context_mock.ShouldNotFail();
    }
}
