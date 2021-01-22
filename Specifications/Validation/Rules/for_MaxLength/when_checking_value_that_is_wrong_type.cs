﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rules;
using Dolittle.Runtime.Validation;
using Dolittle.Runtime.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Validation.Rules.for_MaxLength
{
    public class when_checking_value_that_is_wrong_type
    {
        static MaxLength rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new MaxLength(null, 42);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, 45);

        It should_fail_with_wrong_type_as_reason = () => rule_context_mock.Verify(r => r.Fail(rule, Moq.It.IsAny<object>(), Moq.It.Is<Cause>(_ => _.Reason == ValueRule.ValueTypeMismatch)), Times.Once());
    }
}
