// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Scoping;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the <see cref="Scopes"/> for the <see cref="Type"/> from its defined attributes.
    /// </summary>
    /// <param name="type">The type to get the scope for.</param>
    /// <returns>The scope for the type.</returns>
    public static Scopes GetScope(this Type type)
    {
        var hasPerTenantAttribute = Attribute.IsDefined(type, typeof(PerTenantAttribute));

        return hasPerTenantAttribute ? Scopes.PerTenant
            : Scopes.Global;
    }
}
