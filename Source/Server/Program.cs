// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using BaselineTypeDiscovery;
using Dolittle.Runtime.Configuration.Legacy;
using Dolittle.Runtime.DependencyInversion.Booting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// var services = new AutofacServiceProviderFactory();
var assemblies = AssemblyFinder.FindAssemblies(
    _ => { },
    _ => _.FullName!.StartsWith("Dolittle.Runtime", StringComparison.InvariantCulture) && !_.FullName.Contains("Contracts", StringComparison.InvariantCulture),
    false);
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseAutofac();
builder.Configuration.AddLegacyDolittleFiles();
builder.Services
    .AddRouting()
    .AddControllers();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapControllers();
app.MapGrpcService<Program>();

app.Run();

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
