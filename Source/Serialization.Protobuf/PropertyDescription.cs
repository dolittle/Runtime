// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Dolittle.Runtime.Serialization.Protobuf;

/// <summary>
/// Represents a description of a property, typically found in a <see cref="MessageDescription"/>.
/// </summary>
public class PropertyDescription
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDescription"/> class.
    /// </summary>
    /// <param name="property"><see cref="PropertyInfo"/> representing the actual property.</param>
    /// <param name="name">Specific custom name of the property.</param>
    /// <param name="defaultValue">The default value for the property if not set.</param>
    /// <param name="number">Number representing the property.</param>
    public PropertyDescription(PropertyInfo property, string name = null, object defaultValue = null, int number = 0)
    {
        Property = property;
        Name = name ?? property.Name;
        DefaultValue = defaultValue;
        Number = number == 0 ? GeneratePropertyNumberFromName() : number;
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> for the property.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the position within a <see cref="MessageDescription">message</see>.
    /// </summary>
    /// <remarks>
    /// Positions in Protobuf has a 1 offset.
    /// </remarks>
    public int Number { get; }

    /// <summary>
    /// Gets the default value for the property.
    /// </summary>
    public object DefaultValue { get; }

    int GeneratePropertyNumberFromName()
    {
#pragma warning disable CA5351 // Using unsafe crypto - we're not doing this for security, but providing a predictable hash
        // https://stackoverflow.com/questions/19985273/get-16-bit-hash-of-a-string-in-c-sharp
        using var hasher = MD5.Create();
        var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(Property.Name));
        return BitConverter.ToInt16(data, 0);
#pragma warning restore CA5351
    }
}