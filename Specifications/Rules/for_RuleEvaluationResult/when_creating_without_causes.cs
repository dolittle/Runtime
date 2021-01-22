// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Rules.for_RuleEvaluationResult
{
    public class when_creating_without_causes
    {
        static RuleEvaluationResult result;
        static object instance;

        Establish context = () => instance = new object();

        Because of = () => result = new RuleEvaluationResult(instance);

        It should_be_considered_success = () => result.IsSuccess.ShouldBeTrue();
        It should_be_considered_success_through_implicit_operator = () => ((bool)result).ShouldBeTrue();
    }
}