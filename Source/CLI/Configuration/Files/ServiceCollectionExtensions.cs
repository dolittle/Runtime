// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.CLI.Configuration.Files
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services that handle finding and serialization of configuration files in the file system.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        public static void AddConfigurationFiles(this ServiceCollection services)
        {
            services.AddTransient<IFileProvider>(provider =>
            {
                var cli = provider.GetRequiredService<CommandLineApplication>();
                var workingDirectory = cli.WorkingDirectory;
                return new PhysicalFileProvider(workingDirectory);
            });
            services.AddTransient<ISerializer, Serializer>();
        }
    }
}
