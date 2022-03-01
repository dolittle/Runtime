// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Configuration.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="Microsoft.Extensions.Options.OptionsFactory{TOptions}"/> specific for Dolittle configuration.
/// </summary>
/// <typeparam name="TOptions">The <see cref="Type"/> of the Dolittle configuration.</typeparam>
public class OptionsFactory<TOptions> : Microsoft.Extensions.Options.OptionsFactory<TOptions>
    where TOptions : class
{
    readonly IConfiguration _configuration;
    readonly IEnumerable<ConfigurationObjectDefinition<TOptions>> _definitions;
    readonly IParseConfigurationObjects _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsFactory{TOptions}"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration."/></param>
    /// <param name="definitions">The <see cref="IEnumerable{T}"/> of <see cref="ConfigurationObjectDefinition{TOptions}"/>.</param>
    /// <param name="parser">The <see cref="IParseConfigurationObjects"/>.</param>
    /// <param name="setups">The <see cref="IEnumerable{T}"/> of <see cref="IConfigureOptions{TOptions}"/>.</param>
    /// <param name="postConfigures">The <see cref="IEnumerable{T}"/> of <see cref="IPostConfigureOptions{TOptions}"/>.</param>
    /// <param name="validations">The <see cref="IEnumerable{T}"/> of <see cref="IValidateOptions{TOptions}"/>.</param>
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
            throw new CannotParseConfiguration(typeof(TOptions), section.Path);
        }

        return instance;
    }

    /// <summary>
    /// Gets the configuration section of an <see cref="IConfiguration"/> for a Dolittle configuration.
    /// </summary>
    /// <param name="definition">The <see cref="ConfigurationObjectDefinition{TOptions}"/> of the Dolittle configuration to get the configuration section for.</param>
    /// <returns>The configuration section string.</returns>
    protected virtual string GetConfigurationSection(ConfigurationObjectDefinition<TOptions> definition)
    {
        if (definition.IsPerTenant)
        {
            throw new CannotCreateTenantSpecificConfigurationFromRootContainer();
        }

        return GetConfigurationSectionWithPrefix(definition.Section);
    }

    /// <summary>
    /// Gets the section string prefixed correctly.
    /// </summary>
    /// <param name="section">The configuration section to prefix.</param>
    /// <returns>The correctly prefixed configuration section string.</returns>
    protected string GetConfigurationSectionWithPrefix(string section)
        => $"dolittle:runtime:{section}";
}
