// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Hosting.Microsoft
{
    /// <summary>
    /// Extension methods to the <see cref="IHostBuilder"/> to add Dolittle services.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds Dolittle as the service provider.
        /// </summary>
        /// <remarks>
        /// This runs the Dolittle boot process as part of the application startup.
        /// </remarks>
        /// <param name="builder">The <see cref="IHostBuilder"/> to configure.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder UseDolittle(this IHostBuilder builder)
            => builder.UseServiceProviderFactory(context => new ServiceProviderFactory(context));
    }
}
