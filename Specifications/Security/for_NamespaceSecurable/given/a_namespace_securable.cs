// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Security;

namespace Dolittle.Runtime.Security.Specs.for_NamespaceSecurable.given;

public class a_namespace_securable
{
    protected static NamespaceSecurable namespace_securable;
    protected static object action_with_namespace_match;
    protected static object action_within_another_namespace;

    public a_namespace_securable()
    {
        action_with_namespace_match = new SomeType();
        action_within_another_namespace = new TotallyDifferentNamespace.TypeInDifferentNamespace();

        namespace_securable = new NamespaceSecurable(typeof(SomeType).Namespace);
    }
}