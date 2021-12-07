// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Security.Specs.for_ClaimTypeRule;

[Subject(typeof(ClaimTypeRule))]
public class when_getting_the_rule_description : given.a_claim_type_rule
{
    static string description;
    static string expected_description;

    Establish context = () => expected_description = string.Format(ClaimTypeRule.DescriptionFormat, required_claim);

    Because of = () => description = rule.Description;

    It should_indicate_the_role_that_is_required = () => description.ShouldEqual(expected_description);
}