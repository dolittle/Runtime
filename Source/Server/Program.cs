// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Started Dolittle server");
var host = Host.CreateDefaultBuilder()
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .Build();
    
host.Run();
