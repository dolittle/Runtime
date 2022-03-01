// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Represents the definition of a Dolittle configuration.
/// </summary>
/// <typeparam name="TConfiguration">The <see cref="System.Type"/> of the Dolittle configuration object.</typeparam>
public class ConfigurationObjectDefinition<TConfiguration>
    where TConfiguration : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationObjectDefinition{TOptions}"/> class.
    /// </summary>
    /// <param name="attribute">The <see cref="ConfigurationAttribute"/>.</param>
    public ConfigurationObjectDefinition(ConfigurationAttribute attribute)
        : this(attribute.Section)
    {
        IsPerTenant = false;
    }
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationObjectDefinition{TOptions}"/> class.
    /// </summary>
    /// <param name="attribute">The <see cref="TenantConfigurationAttribute"/>.</param>
    public ConfigurationObjectDefinition(TenantConfigurationAttribute attribute)
        : this(attribute.Section)
    {
        IsPerTenant = true;
    }

    ConfigurationObjectDefinition(string section)
    {
        Section = section;
    }

    /// <summary>
    /// Gets the section where this configuration resides in the <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    public string Section { get; }
    
    /// <summary>
    /// Gets a value indicating whether this Dolittle configuration is per tenant or not.
    /// </summary>
    public bool IsPerTenant { get; }
}
