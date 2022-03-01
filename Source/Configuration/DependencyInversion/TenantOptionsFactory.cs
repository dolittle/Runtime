// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="OptionsFactory{TOptions}"/> for creating <see cref="IOptions{TOptions}"/> for tenant-bound Dolittle configurations.
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public class TenantOptionsFactory<TOptions> : OptionsFactory<TOptions>
    where TOptions : class
{
    readonly TenantId _tenant;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantOptionsFactory{TOptions}"/> class.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/> to create <see cref="IOptions{TOptions}"/> configuration for.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <param name="definitions">The <see cref="IEnumerable{T}"/> of <see cref="ConfigurationObjectDefinition{TOptions}"/>.</param>
    /// <param name="parser">The <see cref="IParseConfigurationObjects"/>.</param>
    /// <param name="setups">The <see cref="IEnumerable{T}"/> of <see cref="IConfigureOptions{TOptions}"/>.</param>
    /// <param name="postConfigures">The <see cref="IEnumerable{T}"/> of <see cref="IPostConfigureOptions{TOptions}"/>.</param>
    /// <param name="validations">The <see cref="IEnumerable{T}"/> of <see cref="IValidateOptions{TOptions}"/>.</param>
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

    /// <inheritdoc />
    protected override string GetConfigurationSection(ConfigurationObjectDefinition<TOptions> definition)
        => definition.IsPerTenant
            ? GetConfigurationSectionWithPrefix($"tenants:{_tenant}:{definition.Section}")
            : base.GetConfigurationSection(definition);
}
