// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Specifications.for_And;

[Subject(typeof(Specification<>))]
public class when_applying_an_and_rule_against_a_instance_satifying_only_one_part : given.rules_and_colored_shapes
{
    static bool is_satisfied;

    Because of = () => is_satisfied = squares.And(green).IsSatisfiedBy(red_square);

    It should_not_be_satisfied = () => is_satisfied.ShouldBeFalse();
}