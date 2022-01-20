// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Runtime.Serialization.Protobuf;

/// <summary>
/// Represents an implementation of <see cref="IPropertyDescriptionBuilder"/>.
/// </summary>
public class PropertyDescriptionBuilder : IPropertyDescriptionBuilder
{
    readonly PropertyInfo _property;
    readonly string _name;
    readonly object _defaultValue;
    readonly int _number;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDescriptionBuilder"/> class.
    /// </summary>
    /// <param name="property"><see cref="PropertyInfo"/> to build for.</param>
    /// <param name="name">Name of the property.</param>
    /// <param name="defaultValue">Default value to use if no value is set.</param>
    /// <param name="number">Protobuf number - index in descriptor.</param>
    public PropertyDescriptionBuilder(PropertyInfo property, string name, object defaultValue, int number)
    {
        _property = property;
        _name = name;
        _defaultValue = defaultValue;
        _number = number;
    }

    /// <summary>
    /// Specify a specific name of the property.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <returns>A new <see cref="PropertyDescriptionBuilder"/> for the chain.</returns>
    public PropertyDescriptionBuilder WithName(string name) => new(_property, name, _defaultValue, _number);

    /// <summary>
    /// Specify a specific default value of the property when a value is not specified.
    /// </summary>
    /// <param name="defaultValue">Default value of the property.</param>
    /// <returns>A new <see cref="PropertyDescriptionBuilder"/> for the chain.</returns>
    public PropertyDescriptionBuilder WithDefaultValue(object defaultValue) => new(_property, _name, defaultValue, _number);

    /// <summary>
    /// Specify a number representing the property.
    /// </summary>
    /// <param name="number">Number for property.</param>
    /// <returns>A new <see cref="PropertyDescriptionBuilder"/> for the chain.</returns>
    public PropertyDescriptionBuilder WithNumber(int number) => new(_property, _name, _defaultValue, number);

    /// <inheritdoc/>
    public PropertyDescription Build()
    {
        return new PropertyDescription(_property, _name, _defaultValue, _number);
    }
}