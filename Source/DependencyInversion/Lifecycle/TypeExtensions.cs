// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Lifecycle;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the <see cref="Lifecycles"/> for the the <see cref="Type"/> from its defined attributes.
    /// </summary>
    /// <param name="type">The type to get the lifecycle for.</param>
    /// <returns>The lifecycle for the type.</returns>
    public static Lifecycles GetLifecycle(this Type type)
    {
        var hasSingletonAttribute = Attribute.IsDefined(type, typeof(SingletonAttribute));
        var hasScopedAttribute = Attribute.IsDefined(type, typeof(ScopedAttribute));

        if (hasSingletonAttribute && hasScopedAttribute)
        {
            throw new TypeHasMultipleLifecycleAttributes(type);
        }

        return hasSingletonAttribute ? Lifecycles.Singleton
            : hasScopedAttribute ? Lifecycles.Scoped
            : Lifecycles.Transient;
    }
}
