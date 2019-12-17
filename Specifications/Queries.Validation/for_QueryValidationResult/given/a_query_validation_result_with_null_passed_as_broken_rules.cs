// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationResult.given
{
    public class a_query_validation_result_with_null_passed_as_broken_rules
    {
        protected static QueryValidationResult result;

        Establish context = () => result = new QueryValidationResult(null);
    }
}
