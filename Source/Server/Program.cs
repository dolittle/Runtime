// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration.Legacy;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Server.Web;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
    .AddMetrics()
    .AddGrpcHost(EndpointVisibility.Private)
    .AddGrpcHost(EndpointVisibility.Public)
    .AddGrpcHost(EndpointVisibility.Management)
    .AddMetricsHost()
    .AddWebHost()
    .Build();

host.Run();
