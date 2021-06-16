// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Scopes;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingScopeBuilder
{
    public class when_building_with_binding_scope_as_singleton : given.a_transient_binding
    {
        static Binding result;

        Because of = () =>
        {
            builder.Singleton();
            result = builder.Build();
        };

        It should_get_scoped_to_singleton = () => result.Scope.ShouldBeOfExactType<Singleton>();
    }
}