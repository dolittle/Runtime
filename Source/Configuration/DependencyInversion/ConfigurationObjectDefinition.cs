// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration.DependencyInversion;

public class ConfigurationObjectDefinition<TOptions>
    where TOptions : class
{
    public ConfigurationObjectDefinition(ConfigurationAttribute attribute)
    {
        Section = attribute.Section;
        IsPerTenant = false;
    }

    public ConfigurationObjectDefinition(TenantConfigurationAttribute attribute)
    {
        Section = attribute.Section;
        IsPerTenant = true;
    }

    public string Section { get; }
    
    public bool IsPerTenant { get; }
}
