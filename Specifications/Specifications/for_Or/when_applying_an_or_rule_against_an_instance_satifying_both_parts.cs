// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Specifications.for_Or;

[Subject(typeof(Specification<>))]
public class when_applying_an_or_rule_against_an_instance_satifying_both_parts : given.rules_and_colored_shapes
{
    static bool is_satisfied;

    Because of = () => is_satisfied = squares.Or(green).IsSatisfiedBy(green_square);

    It should_be_satisfied = () => is_satisfied.ShouldBeTrue();
}