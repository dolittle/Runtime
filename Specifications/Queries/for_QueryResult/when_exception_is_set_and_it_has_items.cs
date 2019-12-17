// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Queries.Specs.for_QueryResult
{
    public class when_exception_is_set_and_it_has_items
    {
        static QueryResult result;

        Establish context = () => result = new QueryResult();

        Because of = () =>
        {
            result.Exception = new ArgumentException(string.Empty);
            result.Items = new object[1];
        };

        It should_not_be_successful = () => result.Success.ShouldBeFalse();
    }
}
