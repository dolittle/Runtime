// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ICanAddTenantServices"/> for adding the configuration DI system. 
/// </summary>
public class TenantServices : ICanAddTenantServices
{
    /// <inheritdoc />
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        services.AddOptions();
        services.Add(ServiceDescriptor.Singleton(typeof(IOptionsFactory<>), typeof(TenantOptionsFactory<>)));
    }
}
