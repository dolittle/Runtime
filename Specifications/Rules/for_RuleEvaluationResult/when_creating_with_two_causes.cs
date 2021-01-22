// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Rules.for_RuleEvaluationResult
{
    public class when_creating_with_two_causes
    {
        static Reason first_reason = Reason.Create("ffa1f234-2345-46c4-b4c3-dda1f73f5c03", "First reason");
        static Reason second_reason = Reason.Create("296bac42-025a-4697-824e-d5019cf3f1c8", "Second reason");
        static Cause first_cause;
        static Cause second_cause;
        static RuleEvaluationResult result;
        static object instance;

        Establish context = () =>
        {
            instance = new object();
            first_cause = first_reason.NoArgs();
            second_cause = second_reason.NoArgs();
        };

        Because of = () => result = new RuleEvaluationResult(instance, first_cause, second_cause);

        It should_be_considered_failed = () => result.IsSuccess.ShouldBeFalse();
        It should_be_considered_failed_through_implicit_operator = () => ((bool)result).ShouldBeFalse();
        It should_hold_both_causes = () => result.Causes.ShouldContainOnly(first_cause, second_cause);
    }
}