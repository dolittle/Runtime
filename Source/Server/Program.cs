// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Configuration.Legacy;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Server.Web;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var host = Host.CreateDefaultBuilder(args)
    .UseDolittleServices()
    .ConfigureHostConfiguration(configuration =>
    {
        configuration.AddJsonFile("appsettings.json");
        configuration.AddJsonFile("runtime.json");
        configuration.AddLegacyDolittleFiles();
        configuration.AddEnvironmentVariables();
        configuration.AddCommandLine(args);
    })
    .AddActorSystem()
    .AddMetrics()
    .AddGrpcHost(EndpointVisibility.Private)
    .AddGrpcHost(EndpointVisibility.Public)
    .AddGrpcHost(EndpointVisibility.Management)
    .AddMetricsHost()
    .AddWebHost()
    .Build();

VerifyConfiguration(host.Services);

host.Run();


static void VerifyConfiguration(IServiceProvider provider)
{
    try
    {
        var config = provider.GetRequiredService<IOptions<EventStoreBackwardsCompatibilityConfiguration>>();
        if (config.Value.Version == EventStoreBackwardsCompatibleVersion.NotSet)
        {
            throw new Exception("Event Store Backwards Compatability Version needs to be set to either 'V6' or 'V7'");
        }
    }
    catch (Exception e)
    {
        provider.GetRequiredService<ILogger<Program>>().LogCritical(e, @"Cannot start Runtime because it is missing the event store backwards compatability configuration.
Make sure that the dolittle:runtime:eventStore:backwardsCompatibility configuration is provided by setting the 'DOLITTLE__RUNTIME__EVENTSTORE__BACKWARDSCOMPATIBILITY__VERSION' environment variable to either V6 (store PartitionId and EventSourceId as Guid) or V7 (store PartitionId and EventSourceId as string)");
        throw;
    }
}
