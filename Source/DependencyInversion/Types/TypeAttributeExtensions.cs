// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Extensions on <see cref="Type"/>.
/// </summary>
public static class TypeAttributeExtensions
{
    /// <summary>
    /// Tries to get a attribute on a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="attribute">The outputted attribute.</param>
    /// <typeparam name="TAttribute">The <see cref="Type"/> of the attribute.</typeparam>
    /// <returns>True if <see cref="Type"/> has the attribute, false if not.</returns>
    public static bool TryGetAttribute<TAttribute>(this Type type, out TAttribute attribute)
        where TAttribute : Attribute
    {
        if (!Attribute.IsDefined(type, typeof(TAttribute)))
        {
            attribute = default;
            return false;
        }

        attribute = Attribute.GetCustomAttribute(type, typeof(TAttribute)) as TAttribute;
        return attribute != default;
    }
}
