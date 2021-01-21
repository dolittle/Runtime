// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptAs
{
    [Subject(typeof(ConceptAs<>))]
    public class when_comparing_on_generic_method : given.comparable_concepts
    {
        Because of = () =>
        {
            compare_least_to_most = least.CompareTo(most);
            compare_least_to_middle = least.CompareTo(middle);
            compare_least_to_self = least.CompareTo(least);
            compare_middle_to_least = middle.CompareTo(least);
            compare_middle_to_most = middle.CompareTo(most);
            compare_middle_to_self = middle.CompareTo(middle);
            compare_most_to_least = most.CompareTo(least);
            compare_most_to_middle = most.CompareTo(middle);
            compare_most_to_self = most.CompareTo(most);
            compare_most_to_another_instance_of_most = most.CompareTo(another_instance_of_most);
        };

        It determines_least_is_less_than_most = () => compare_least_to_most.ShouldEqual(given.comparable_concepts.LESS_THAN);
        It determines_least_is_less_than_middle = () => compare_least_to_middle.ShouldEqual(given.comparable_concepts.LESS_THAN);
        It determines_least_is_equal_to_itself = () => compare_least_to_self.ShouldEqual(given.comparable_concepts.EQUAL);
        It determines_middle_is_greater_than_least = () => compare_middle_to_least.ShouldEqual(given.comparable_concepts.GREATER_THAN);
        It determines_middle_is_less_than_most = () => compare_middle_to_most.ShouldEqual(given.comparable_concepts.LESS_THAN);
        It determines_middle_is_equal_to_itself = () => compare_middle_to_self.ShouldEqual(given.comparable_concepts.EQUAL);
        It determines_most_is_greater_than_least = () => compare_most_to_least.ShouldEqual(given.comparable_concepts.GREATER_THAN);
        It determines_most_is_greater_than_middle = () => compare_most_to_middle.ShouldEqual(given.comparable_concepts.GREATER_THAN);
        It determines_most_is_equal_to_itself = () => compare_most_to_self.ShouldEqual(given.comparable_concepts.EQUAL);
        It determines_most_is_equal_to_another_instance_of_most = () => compare_most_to_another_instance_of_most.ShouldEqual(given.comparable_concepts.EQUAL);
    }
}