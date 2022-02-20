// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Configuration.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

public class OptionsFactory<TOptions> : Microsoft.Extensions.Options.OptionsFactory<TOptions>
    where TOptions : class
{
    readonly IConfiguration _configuration;
    readonly IEnumerable<ConfigurationObjectDefinition<TOptions>> _definitions;
    readonly IParseConfigurationObjects _parser;

    public OptionsFactory(
        IConfiguration configuration,
        IEnumerable<ConfigurationObjectDefinition<TOptions>> definitions,
        IParseConfigurationObjects parser,
        IEnumerable<IConfigureOptions<TOptions>> setups, 
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
        IEnumerable<IValidateOptions<TOptions>> validations)
        : base(setups, postConfigures, validations)
    {
        _configuration = configuration;
        _definitions = definitions;
        _parser = parser;
    }

    /// <inheritdoc />
    protected override TOptions CreateInstance(string name)
    {
        var definition = _definitions.FirstOrDefault();
        if (definition == default)
        {
            return base.CreateInstance(name);
        }

        var section = _configuration.GetSection(GetConfigurationSection(definition));
        if (!_parser.TryParseFrom<TOptions>(section, out var instance))
        {
            throw new Exception("Cannot Parse configuration");
        }

        return instance;
    }

    protected virtual string GetConfigurationSection(ConfigurationObjectDefinition<TOptions> definition)
    {
        if (definition.IsPerTenant)
        {
            throw new Exception("Cannot create tenant-specific configuration from the root container");
        }

        return GetConfigurationSectionWithPrefix(definition.Section);
    }

    protected string GetConfigurationSectionWithPrefix(string section)
        => $"dolittle:runtime:{section}";
}
