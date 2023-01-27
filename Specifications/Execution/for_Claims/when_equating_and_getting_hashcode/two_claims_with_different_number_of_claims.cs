// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_Claims.when_equating_and_getting_hashcode;

[Subject(typeof(Claims), nameof(Equals))]
public class two_claims_with_different_number_of_claims
{
    static Claims first;
    static Claims second;

    static bool is_equal_by_method;
    static bool is_equal_by_operator;
    static bool is_not_equal_by_operator;
    static bool hash_code_is_equal;

    Establish context = () =>
    {
        var list = new List<Claim>
        {
            new("4", "4", "4"),
            new("1", "1", "1"),
            new("2", "2", "2"),
            new("3", "3", "3")
        };
        first = new Claims(list.ToArray());
        second = new Claims(list.Take(2).ToArray());
    };

    Because of = () =>
    {
        is_equal_by_method = first.Equals(second);
        is_equal_by_operator = first == second;
        is_not_equal_by_operator = first != second;
        hash_code_is_equal = first.GetHashCode() == second.GetHashCode();
    };

    It should_not_be_equal_when_using_the_Equals_method = () => is_equal_by_method.Should().BeFalse();
    It should_not_be_equal_when_using_the_equals_operator = () => is_equal_by_operator.Should().BeFalse();
    It should_not_be_equal_when_using_the_not_equals_operator = () => is_not_equal_by_operator.Should().BeTrue();
    It should_not_have_the_same_hashcode = () => hash_code_is_equal.Should().BeFalse();
}