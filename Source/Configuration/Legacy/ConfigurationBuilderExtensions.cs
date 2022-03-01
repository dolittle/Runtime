// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration.Legacy;

/// <summary>
/// Extension methods for <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="LegacyConfigurationSource"/> <see cref="IConfigurationSource"/> to the <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>Tbe builder for continuation.</returns>
    public static IConfigurationBuilder AddLegacyDolittleFiles(this IConfigurationBuilder builder)
        => builder.Add(new LegacyConfigurationSource());
}
