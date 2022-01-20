// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Dolittle.Runtime.Immutability;

/// <summary>
/// Holds extension methods for working with immutables.
/// </summary>
public static class ImmutableExtensions
{
    const BindingFlags PublicInstancePropertyFlag = BindingFlags.Public | BindingFlags.Instance;

    /// <summary>
    /// Check if a type is immutable by virtue of it having public properties or fields that can be written to.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if it is immutable, false if not.</returns>
    /// <remarks>
    /// Immutability is a difficult concept in C#.  Things can be changed via reflection, fields rather than properties, private setters, etc. static
    /// We are taking a deliberately limited view of immutability.  In this case it simply means an object that has no properties with setters (public or private)
    /// This is not intended to be an indication that the object is truly immutable, instead it's to guide the instantiation strategy when we create and hydrate it
    /// from a serialized form.
    /// </remarks>
    public static bool IsImmutable(this Type type)
    {
        var writeableProperties = type.GetWriteableProperties();
        var writeableFields = type.GetWriteableFields();

        return writeableProperties.Length == 0 && writeableFields.Length == 0;
    }

    /// <summary>
    /// Check if a type is really immutable and if it's not, throw the appropriate exception.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    public static void ShouldBeImmutable(this Type type)
    {
        var writeableProperties = type.GetWriteableProperties();
        if (writeableProperties.Length > 0) throw new WriteableImmutablePropertiesFound(type, writeableProperties);

        var writeableFields = type.GetWriteableFields();
        if (writeableFields.Length > 0) throw new WriteableImmutableFieldsFound(type, writeableFields);
    }

    /// <summary>
    /// Get the writeable properties - if any - on a specific type.
    /// For init-only properties:
    /// https://www.meziantou.net/csharp9-init-only-properties-are-not-read-only.htm
    /// </summary>
    /// <param name="type"><see cref="Type"/> to get from.</param>
    /// <returns>Writeable <see cref="PropertyInfo">properties</see>.</returns>
    public static PropertyInfo[] GetWriteableProperties(this Type type)
        => type.GetProperties(PublicInstancePropertyFlag)
            .Where(_ => _.CanWrite)
            .Where(_ => !IsInitSetter(_))
            .ToArray();

    /// <summary>
    /// Get the writeable fields - if any - on a specific type.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to get from.</param>
    /// <returns>Writeable <see cref="FieldInfo">fields</see>.</returns>
    public static FieldInfo[] GetWriteableFields(this Type type)
        => type
            .GetFields(PublicInstancePropertyFlag)
            .Where(_ => (_.Attributes & FieldAttributes.InitOnly) == 0)
            .ToArray();

    static bool IsInitSetter(PropertyInfo property)
        => property.SetMethod.ReturnParameter
            .GetRequiredCustomModifiers()
            .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
}