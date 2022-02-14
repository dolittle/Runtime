// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Configuration.Legacy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var services = new AutofacServiceProviderFactory();

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(services)
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddLegacyDolittleFiles();
        configuration.AddJsonFile("appsettings.json");
    })
    .Build();

await host.RunAsync();
