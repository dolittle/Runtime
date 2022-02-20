// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration.ConfigurationObjects;
using Dolittle.Runtime.Configuration.Legacy;
using Dolittle.Runtime.DependencyInversion.Building;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddRouting()
    .AddControllers();
builder.Configuration.AddLegacyDolittleFiles();
builder.Services.AddDolittleConfigurations(builder.Configuration);
builder.Services.AddGrpc();
builder.Host.UseDolittleServices();

var app = builder.Build();

var services = app.Services;
var dolittleConfig = services.GetRequiredService<DolittleConfigurations>();

// app.MapControllers();
// app.MapGrpcService<Program>();

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
