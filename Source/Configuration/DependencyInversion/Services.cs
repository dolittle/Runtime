// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ICanAddServices"/> for plugging Dolittle configuration files into the Microsoft configuration system.
/// </summary>
public class Services : ICanAddServices 
{
    /// <inheritdoc />
    public void AddTo(IServiceCollection services)
    {
        services.AddOptions();
        services.Add(ServiceDescriptor.Singleton(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
    }
}
