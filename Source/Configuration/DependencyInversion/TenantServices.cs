// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

public class TenantServices : ICanAddTenantServices
{
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        Console.WriteLine($"Adding tenant options for {tenant}");
        services.AddOptions();
        services.Add(ServiceDescriptor.Singleton(typeof(IOptionsFactory<>), typeof(TenantOptionsFactory<>)));
    }
}
