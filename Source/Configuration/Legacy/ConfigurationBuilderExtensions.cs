// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration.Legacy;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddLegacyDolittleFiles(this IConfigurationBuilder builder)
        => builder.Add(new LegacyConfigurationSource());
}
