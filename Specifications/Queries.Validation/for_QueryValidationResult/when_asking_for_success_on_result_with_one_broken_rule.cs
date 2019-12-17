// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationResult
{
    public class when_asking_for_success_on_result_with_one_broken_rule : given.a_query_validation_result_with_one_broken_rule
    {
        static bool success;

        Because of = () => success = result.Success;

        It should_be_considered_successful = () => success.ShouldBeFalse();
    }
}
