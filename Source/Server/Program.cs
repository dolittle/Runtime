// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using BaselineTypeDiscovery;

// var services = new AutofacServiceProviderFactory();
// var stopWatch = new Stopwatch();
// stopWatch.Start();
var assemblies = AssemblyFinder.FindAssemblies(
    _ => { },
    _ => _.FullName!.StartsWith("Dolittle.Runtime", StringComparison.InvariantCulture) && !_.FullName.Contains("Contracts", StringComparison.InvariantCulture),
    false);
// stopWatch.Stop();
// Console.WriteLine($"Found {assemblies.Count()} dolittle assemblies, took {stopWatch.Elapsed} {stopWatch.Elapsed.Milliseconds} {stopWatch.ElapsedTicks}");
Console.WriteLine($"{AppDomain.CurrentDomain.GetAssemblies().Length}");
return;
// var builder = WebApplication.CreateBuilder(args);
//
// builder.Host.UseServiceProviderFactory(services);
// builder.Host.ConfigureContainer<ContainerBuilder>(_ =>
// {
//     foreach (var assembly in assemblies)
//     {
//         _.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
//         _.RegisterAssemblyModules(assembly);   
//     }
// });
// builder.Configuration.AddLegacyDolittleFiles();
// builder.Services
//     .AddRouting()
//     .AddControllers();
// builder.Services.AddGrpc();
//
// var app = builder.Build();
//
// app.MapControllers();
// // app.MapGrpcService<Program>();
//
// app.Run();

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
