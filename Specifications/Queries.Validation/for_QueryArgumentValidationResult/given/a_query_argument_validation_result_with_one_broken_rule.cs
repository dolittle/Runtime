// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Rules;
using Machine.Specifications;
using Moq;

namespace Dolittle.Queries.Validation.Specs.for_QueryArgumentValidationResult.given
{
    public class a_query_argument_validation_result_with_one_broken_rule
    {
        protected static QueryArgumentValidationResult result;
        protected static Mock<IRule> rule_mock;
        protected static object instance;
        protected static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule_mock = new Mock<IRule>();
            instance = new object();
            rule_context_mock = new Mock<IRuleContext>();

            result = new QueryArgumentValidationResult(new[] { new BrokenRule(rule_mock.Object, instance, rule_context_mock.Object) });
        };
    }
}
