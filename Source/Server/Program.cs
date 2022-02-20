// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Hosting;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .UseDolittleServices()
    .AddGrpcHost(EndpointVisibility.Private, 50053)
    .Build();


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
