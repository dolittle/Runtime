// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Docker.DotNet;
using Dolittle.Runtime.CLI.Runtime.EventHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Runtime
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRuntimeServices(this IServiceCollection services)
        {
            services.AddTransient<ICanCreateClients, ClientFactory>();
            services.AddTransient<ICanLocateRuntimes, RuntimeLocator>();
            services.AddTransient<ICanDiscoverRuntimeAddresses, DockerRuntimeAddresses>();
            services.AddSingleton<IDockerClient>(provider =>
                new DockerClientConfiguration().CreateClient());
            
            services.AddEventHandlerServices();
        }
    }
}