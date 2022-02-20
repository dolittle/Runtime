// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Defines an attribute that can be used to define a name of a <see cref="IConfigurationObject"/>.
/// </summary>
/// <remarks>
/// This name can be used as the basis of a configuration section or even as a file name.
/// The decision is up to each <see cref="ICanProvideConfigurationObjects">provider</see>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigAttribute"/> class.
    /// </summary>
    /// <param name="section">Name to use.</param>
    public ConfigAttribute(string section)
    {
        Section = section;
    }

    /// <summary>
    /// Gets the sub-section where this configuration belongs in the dolittle:runtime <see cref="IConfiguration"/>.
    /// </summary>
    public string Section { get; }
}
