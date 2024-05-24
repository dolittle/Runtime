// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Bootstrap.Hosting;
using Dolittle.Runtime.Configuration;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Diagnostics.OpenTelemetry;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Server.Web;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var configBuilder = new ConfigurationBuilder();
configBuilder.AddJsonFile("appsettings.json");
configBuilder.AddDolittleFiles();
configBuilder.AddEnvironmentVariables();
configBuilder.AddCommandLine(args);
var config = configBuilder.Build();

var host = Host.CreateDefaultBuilder(args)
    .UseDolittleServices()
    .ConfigureHostConfiguration(configuration => configuration.AddConfiguration(config))
    .ConfigureOpenTelemetry(config)
    .AddActorSystem()
    .AddMetrics()
    .AddGrpcHost(EndpointVisibility.Private)
    .AddGrpcHost(EndpointVisibility.Public)
    .AddGrpcHost(EndpointVisibility.Management)
    .AddGrpcWebHost(EndpointVisibility.ManagementWeb)
    .AddMetricsHost()
    .AddWebHost()
    .Build();

VerifyConfiguration(host.Services);
await host.PerformBootstrap().ConfigureAwait(false);
host.Run();

static void VerifyConfiguration(IServiceProvider provider)
{
    var logger = provider.GetRequiredService<ILogger<Program>>();
    try
    {
        var config = provider.GetRequiredService<IOptions<TenantsConfiguration>>();
        if (!config.Value.Any())
        {
            logger.LogWarning("No tenants are configured in the Runtime. Without any tenants the Runtime will not function properly.");
        }
    }
    catch (Exception e)
    {
        logger.LogError(e, "It seems like the Runtime is missing its 'tenants' configuration. Without any tenants the Runtime will no function properly.");
        throw;
    }
}
