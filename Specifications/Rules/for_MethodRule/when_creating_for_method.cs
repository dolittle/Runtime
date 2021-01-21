// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Rules.for_MethodRule
{
    public class when_creating_for_method
    {
        static RuleEvaluationResult RuleMethod()
        {
            return RuleEvaluationResult.Success;
        }

        const string rule_name = "This is the rule";

        static MethodRule rule;

        Because of = () => rule = new MethodRule(rule_name, RuleMethod);

        It should_hold_name = () => rule.Name.ShouldEqual(rule_name);
    }
}