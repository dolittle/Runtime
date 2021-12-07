// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Security.Specs.for_ClaimTypeRule;

[Subject(typeof(ClaimTypeRule))]
public class when_authorizing_with_user_who_does_not_have_role : given.a_claim_type_rule
{
    static bool result;

    Because of = () => result = rule.IsAuthorized(new object());

    It should_not_be_authorized = () => result.ShouldBeFalse();
}