// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Represents an implementation of <see cref="ICanAddServicesForTypesWith{TAttribute}"/> that adds gRPC services to the DI container.
/// </summary>
public class Services : ICanAddServicesForTypesWith<ServiceAttribute>
{
    /// <inheritdoc />
    public void AddServiceFor(Type type, ServiceAttribute attribute, IServiceCollection services)
    {
        Console.WriteLine($"Registering gRPC service from {type}");
        services.AddSingleton(new ServiceDefinition(attribute.Visibility, type));
        services.AddSingleton(type);
    }
}
