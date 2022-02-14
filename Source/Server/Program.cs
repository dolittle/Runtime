// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Server.Configurations;
using Dolittle.Runtime.Server.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Started Dolittle server");
var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(_ => _.AddJsonFile(Path.Combine(".dolittle", "endpoints.json")))
    .ConfigureAppConfiguration(_ => _.Add(new DolittleConfigurationSource()))
    // .ConfigureAppConfiguration(_ => _.AddJsonFile(_ => _.))
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    // .ConfigureServices()
    .ConfigureGrpcServers()
    .Build();

    
host.Run();
