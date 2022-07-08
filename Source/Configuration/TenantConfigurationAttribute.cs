// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Indicates that the type should be registered as a configuration object in a per-tenant DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class TenantConfigurationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantConfigurationAttribute"/> class.
    /// </summary>
    /// <param name="section">The configuration section to parse the object from, excluding the "dolittle:runtime" prefix</param>
    public TenantConfigurationAttribute(params string[] section)
    {
        Section = ConfigurationPath.Combine(section);
    }
    
    /// <summary>
    /// Gets the configuration section to parse the configuration object from.
    /// </summary>
    /// <remarks>
    /// Excluding the "dolittle:runtime" section prefix.
    /// </remarks>
    public string Section { get; }
}
