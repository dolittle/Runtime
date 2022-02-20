// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Types;

public static class TypeAttributeExtensions
{
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
