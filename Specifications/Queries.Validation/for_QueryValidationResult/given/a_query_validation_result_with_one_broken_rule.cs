// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Rules;
using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationResult.given
{
    public class a_query_validation_result_with_one_broken_rule : all_dependencies
    {
        protected static QueryValidationResult result;

        Establish context = () => result = new QueryValidationResult(new[] { new BrokenRule(rule_mock.Object, instance, rule_context_mock.Object) });
    }
}
