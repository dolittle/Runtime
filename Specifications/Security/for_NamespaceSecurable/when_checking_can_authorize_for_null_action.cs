// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Security.Specs.for_NamespaceSecurable
{
    [Subject(typeof(NamespaceSecurable))]
    public class when_checking_can_authorize_for_null_action : given.a_namespace_securable
    {
        static bool authorized;

        Because of = () => authorized = namespace_securable.CanAuthorize(null);

        It should_not_be_authorized = () => authorized.ShouldBeFalse();
    }
}