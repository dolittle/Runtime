// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Docker.DotNet;
using Dolittle.Runtime.CLI.Runtime.Aggregates;
using Dolittle.Runtime.CLI.Runtime.Configuration;
using Dolittle.Runtime.CLI.Runtime.EventHandlers;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.CLI.Runtime.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Runtime;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to management of Runtimes.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddRuntimeServices(this IServiceCollection services)
    {
        services.AddTransient<ICanCreateClients, ClientFactory>();
        services.AddTransient<ICanLocateRuntimes, RuntimeLocator>();
        services.AddTransient<ICanDiscoverRuntimeAddresses, DockerRuntimeAddresses>();
        services.AddSingleton<IDockerClient>(provider =>
            new DockerClientConfiguration().CreateClient());
            
        services.AddAggregatesServices();
        services.AddConfigurationServices();
        services.AddEventHandlerServices();
        services.AddEventTypesServices();
        services.AddEventsProcessingServices();
        services.AddProjectionServices();
    }
}
