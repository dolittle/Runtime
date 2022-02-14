// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Configuration.Legacy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var services = new AutofacServiceProviderFactory();

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(services)
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddLegacyDolittleFiles();
        configuration.AddJsonFile("appsettings.json");
    })
    .ConfigureServices(services =>
    {
        services.AddRouting();
    })
    .ConfigureWebHost(_ =>
    {
        _.UseKestrel(_ => _.Listen(IPAddress.Any, 50052));
        _.Configure(_ =>
        {
            _.UseRouting();
            _.UseEndpoints(_ =>
            {
                _.MapGet("/", () => "Hello from first host!");
            });
        });
    })
    .ConfigureWebHost(_ =>
    {
        _.UseKestrel(_ => _.Listen(IPAddress.Any, 50053));
        _.Configure(_ =>
        {
            _.UseRouting();
            _.UseEndpoints(_ =>
            {
                _.MapGet("/", () => "Hello from second host!");
            });
        });
    })
    .Build();

await host.RunAsync();
