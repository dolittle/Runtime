// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

public class Services : ICanAddServices 
{
    public void AddTo(IServiceCollection services)
    {
        Console.WriteLine("Adding Options things");
        services.AddOptions();
        services.Add(ServiceDescriptor.Singleton(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
    }
}
