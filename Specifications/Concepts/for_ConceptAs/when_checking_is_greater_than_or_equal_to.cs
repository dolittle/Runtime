// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptAs
{
    [Subject(typeof(ConceptAs<>))]
    public class when_checking_is_greater_than_or_equal_to : given.test_concepts
    {
        static bool least_is_greater_than_or_equal_to_most;
        static bool least_is_greater_than_or_equal_to_middle;
        static bool least_is_greater_than_or_equal_to_self;
        static bool least_is_greater_than_or_equal_to_primitive_value;
        static bool middle_is_greater_than_or_equal_to_least;
        static bool middle_is_greater_than_or_equal_to_most;
        static bool middle_is_greater_than_or_equal_to_self;
        static bool middle_is_greater_than_or_equal_to_primitive_value;
        static bool most_is_greater_than_or_equal_to_least;
        static bool most_is_greater_than_or_equal_to_middle;
        static bool most_is_greater_than_or_equal_to_self;
        static bool most_is_greater_than_or_equal_to_primitive_value;

        Because of = () =>
        {
            least_is_greater_than_or_equal_to_most = least >= most;
            least_is_greater_than_or_equal_to_middle = least >= middle;
            least_is_greater_than_or_equal_to_self = least >= least;
            least_is_greater_than_or_equal_to_primitive_value = least >= least.Value;
            middle_is_greater_than_or_equal_to_least = middle >= least;
            middle_is_greater_than_or_equal_to_most = middle >= most;
            middle_is_greater_than_or_equal_to_self = middle >= middle;
            middle_is_greater_than_or_equal_to_primitive_value = middle >= middle.Value;
            most_is_greater_than_or_equal_to_least = most >= least;
            most_is_greater_than_or_equal_to_middle = most >= middle;
            most_is_greater_than_or_equal_to_self = most >= most;
            most_is_greater_than_or_equal_to_primitive_value = most >= most.Value;
        };

        It determines_least_is_not_greater_than_or_equal_to_most = () => least_is_greater_than_or_equal_to_most.ShouldBeFalse();
        It determines_least_is_not_greater_than_or_equal_to_middle = () => least_is_greater_than_or_equal_to_middle.ShouldBeFalse();
        It determines_least_is_not_greater_than_or_equal_to_self = () => least_is_greater_than_or_equal_to_self.ShouldBeTrue();
        It determines_least_is_not_greater_than_or_equal_to_primitive_value = () => least_is_greater_than_or_equal_to_primitive_value.ShouldBeTrue();
        It determines_middle_is_not_greater_than_or_equal_to_most = () => middle_is_greater_than_or_equal_to_most.ShouldBeFalse();
        It determines_middle_is_greater_than_or_equal_to_least = () => middle_is_greater_than_or_equal_to_least.ShouldBeTrue();
        It determines_middle_is_not_greater_than_or_equal_to_self = () => middle_is_greater_than_or_equal_to_self.ShouldBeTrue();
        It determines_middle_is_not_greater_than_or_equal_to_primitive_value = () => middle_is_greater_than_or_equal_to_primitive_value.ShouldBeTrue();
        It determines_most_is_greater_than_or_equal_to_least = () => most_is_greater_than_or_equal_to_least.ShouldBeTrue();
        It determines_most_is_greater_than_or_equal_to_middle = () => most_is_greater_than_or_equal_to_middle.ShouldBeTrue();
        It determines_most_is_not_greater_than_or_equal_to_self = () => most_is_greater_than_or_equal_to_self.ShouldBeTrue();
        It determines_most_is_not_greater_than_or_equal_to_primitive_value = () => most_is_greater_than_or_equal_to_primitive_value.ShouldBeTrue();
    }
}