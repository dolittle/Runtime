// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Defines an attribute that can be used to define a name of a <see cref="IConfigurationObject"/>.
/// </summary>
/// <remarks>
/// This name can be used as the basis of a configuration section or even as a file name.
/// The decision is up to each <see cref="ICanProvideConfigurationObjects">provider</see>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class NameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NameAttribute"/> class.
    /// </summary>
    /// <param name="name">Name to use.</param>
    public NameAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name to be associated with the <see cref="IConfigurationObject"/>.
    /// </summary>
    public string Name { get; }
}
