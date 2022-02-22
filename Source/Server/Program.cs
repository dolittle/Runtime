﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration.Legacy;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .UseDolittleServices()
    .ConfigureHostConfiguration(configuration =>
    {
        configuration.AddLegacyDolittleFiles();
    })
    .AddMetrics()
    .AddGrpcHost(EndpointVisibility.Private)
    .AddMetricsHost()
    .Build();

host.Run();
