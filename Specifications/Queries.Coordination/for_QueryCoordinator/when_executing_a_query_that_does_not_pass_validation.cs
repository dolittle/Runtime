// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Queries.Validation;
using Dolittle.Rules;
using Machine.Specifications;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class when_executing_a_query_that_does_not_pass_validation : given.a_query_coordinator
    {
        static QueryForKnownProvider query;
        static PagingInfo paging;
        static QueryResult result;

        Establish context = () =>
        {
            query = new QueryForKnownProvider();
            paging = new PagingInfo();

            validation_result = new QueryValidationResult(new[] { new BrokenRule(null, null, null) });

            query_validator.Setup(c => c.Validate(query)).Returns(validation_result);
        };

        Because of = async () => result = await coordinator.Execute(query, paging).ConfigureAwait(false);

        It should_not_be_executing_query = () => query.QueryPropertyCalled.ShouldBeFalse();
        It should_have_hold_an_empty_items_array = () => result.Items.ShouldBeEmpty();
    }
}
