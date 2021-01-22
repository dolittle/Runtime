// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Specifications.for_Specification
{
    [Subject(typeof(Specification<>))]
    public class when_applying_a_simple_rule_against_a_collection : given.rules_and_colored_shapes
    {
        static IEnumerable<ColoredShape> colored_shapes;

        Because of = () => colored_shapes = squares.SatisfyingElementsFrom(my_colored_shapes);

        It should_contain_only_squares = () => colored_shapes.All(r => r.Shape == "Square").ShouldBeTrue();
    }
}