// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.DependencyInversion.Building;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Adds the Dolittle DI container discovery process and service providers to the host.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to modify.</param>
    /// <returns>The <see cref="IHostBuilder"/> for continuation.</returns>
    public static IHostBuilder UseDolittleServices(this IHostBuilder builder)
        => builder.UseServiceProviderFactory(new ServiceProviderFactory());
}
