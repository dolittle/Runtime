// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Rules.for_RuleContext
{
    public class when_failing_with_two_callbacks_registered
    {
        static RuleContext rule_context;
        static Mock<IRule> rule_mock;
        static object instance;
        static Reason reason;
        static Cause cause;

        static Mock<RuleFailed> first_failed_callback;
        static Mock<RuleFailed> second_failed_callback;

        Establish context = () =>
        {
            rule_mock = new Mock<IRule>();
            instance = new object();
            reason = Reason.Create(Guid.NewGuid(), "Some reason");
            cause = reason.NoArgs();
            rule_context = new RuleContext(instance);

            first_failed_callback = new Mock<RuleFailed>();
            rule_context.OnFailed(first_failed_callback.Object);

            second_failed_callback = new Mock<RuleFailed>();
            rule_context.OnFailed(second_failed_callback.Object);
        };

        Because of = () => rule_context.Fail(rule_mock.Object, instance, cause);

        It should_call_first_callback = () => first_failed_callback.Verify(c => c(rule_mock.Object, instance, cause), Moq.Times.Once());
        It should_call_second_callback = () => second_failed_callback.Verify(c => c(rule_mock.Object, instance, cause), Moq.Times.Once());
    }
}
