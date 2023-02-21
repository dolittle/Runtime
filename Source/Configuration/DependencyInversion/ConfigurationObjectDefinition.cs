// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Represents the definition of a Dolittle configuration.
/// </summary>
/// <typeparam name="TConfiguration">The <see cref="System.Type"/> of the Dolittle configuration object.</typeparam>
public class ConfigurationObjectDefinition<TConfiguration> : IAmAConfigurationObjectDefinition
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
        ConfigurationObjectType = typeof(TConfiguration);
    }

    /// <inheritdoc />
    public string Section { get; }

    /// <inheritdoc />
    public bool IsPerTenant { get; }

    /// <inheritdoc />
    public Type ConfigurationObjectType { get; }
}
