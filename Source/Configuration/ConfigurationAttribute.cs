// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Indicates that the type should be registered as a configuration object in a DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ConfigurationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationAttribute"/> class.
    /// </summary>
    /// <param name="section">The configuration section to parse the object from, excluding the "dolittle:runtime" prefix</param>
    public ConfigurationAttribute(params string[] section)
    {
        Section = string.Join(':', section);
    }
    
    /// <summary>
    /// Gets the configuration section to parse the configuration object from.
    /// </summary>
    /// <remarks>
    /// Excluding the "dolittle:runtime" section prefix.
    /// </remarks>
    public string Section { get; }
}
