// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Extensions on <see cref="Type"/>.
/// </summary>
public static class TypeAttributeExtensions
{
    /// <summary>
    /// Gets the defined attributes of a specific type, on a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <returns>The found attributes of the specified type.</returns>
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Type type)
        where TAttribute : Attribute
        => Attribute.GetCustomAttributes(type, typeof(TAttribute)).Cast<TAttribute>();
}
