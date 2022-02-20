// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

public class TenantOptionsFactory<TOptions> : OptionsFactory<TOptions>
    where TOptions : class
{
    readonly TenantId _tenant;

    public TenantOptionsFactory(
        TenantId tenant,
        IConfiguration configuration,
        IEnumerable<ConfigurationObjectDefinition<TOptions>> definitions,
        IParseConfigurationObjects parser,
        IEnumerable<IConfigureOptions<TOptions>> setups, 
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
        IEnumerable<IValidateOptions<TOptions>> validations)
        : base(configuration, definitions, parser, setups, postConfigures, validations)
    {
        _tenant = tenant;
    }

    protected override string GetConfigurationSection(ConfigurationObjectDefinition<TOptions> definition)
        => definition.IsPerTenant
            ? GetConfigurationSectionWithPrefix($"tenants:{_tenant}:{definition.Section}")
            : base.GetConfigurationSection(definition);
}
