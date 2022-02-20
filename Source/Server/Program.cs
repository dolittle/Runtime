// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Building;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = Host.CreateDefaultBuilder(args)
    .UseDolittleServices()
    .Build();

var tenantServices = host.Services.GetRequiredService<ITenantServiceProviders>().ForTenant("d06fd876-cac5-4bf5-a3ae-8ce2a8e78a5b");
var endpoints = tenantServices.GetRequiredService<IOptions<EndpointsConfiguration>>();

Console.WriteLine(endpoints.Value.Keys.Count);


// app.MapControllers();
// app.MapGrpcService<Program>();

host.Run();

// var host = Host.CreateDefaultBuilder(args)
//     .UseServiceProviderFactory(services)
//     .ConfigureAppConfiguration(configuration =>
//     {
//         configuration.AddLegacyDolittleFiles();
//         configuration.AddJsonFile("appsettings.json");
//     })
//     .ConfigureServices(services =>
//     {
//         services.AddRouting();
//         services.AddGrpc();
//     })
    // .ConfigureWebHost(_ =>
    // {
    //     _.UseKestrel(_ => _.Listen(IPAddress.Any, 50052));
    //     _.Configure(_ =>
    //     {
    //         _.UseRouting();
    //         _.UseEndpoints(_ =>
    //         {
    //             _.MapGet("/", () => "Hello from first host!");
    //         });
    //     });
    // })
    // .ConfigureWebHost(_ =>
    // {
    //     _.UseKestrel(_ => _.Listen(IPAddress.Any, 50053));
    //     _.Configure(_ =>
    //     {
    //         _.UseRouting();
    //         _.UseEndpoints(_ =>
    //         {
    //             _.MapGet("/", () => "Hello from second host!");
    //         });
    //     });
    // })
//     .Build();
//
// await host.RunAsync();
