// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Specifications.for_Negative
{
    [Subject(typeof(Specification<>))]
    public class when_applying_a_not_rule_against_a_collection : given.rules_and_colored_shapes
    {
        static IEnumerable<ColoredShape> satisfied_shapes;
        static IEnumerable<ColoredShape> the_not_greens;

        Establish context = () => the_not_greens = my_colored_shapes.Where(s => s.Color != "Green").AsEnumerable();

        Because of = () => satisfied_shapes = Is.Not(green).SatisfyingElementsFrom(my_colored_shapes);

        It should_have_all_instances_not_satisfying_the_rule = () => satisfied_shapes.ShouldContainOnly(the_not_greens);
    }
}