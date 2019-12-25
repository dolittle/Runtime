// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryArgumentValidationResult.given
{
    public class a_query_argument_validation_result_with_null_passed_as_broken_rules
    {
        protected static QueryArgumentValidationResult result;

        Establish context = () => result = new QueryArgumentValidationResult(null);
    }
}
