// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Security.Specs.for_RoleRule;

[Subject(typeof(RoleRule))]
public class when_authorizing_with_user_who_does_not_have_role : given.a_rule_role
{
    static bool is_authorized;

    Because of = () => is_authorized = rule.IsAuthorized(new object());

    It should_not_be_authorized = () => is_authorized.ShouldBeFalse();
}