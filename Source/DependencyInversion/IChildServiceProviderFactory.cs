// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that can create a <see cref="IServiceProvider"/> by adding services to the current DI container.
/// </summary>
public interface IChildServiceProviderFactory
{
    /// <summary>
    /// Creates a new child <see cref="IServiceProvider"/> from the current DI container and adds the provided <see cref="IServiceCollection"/> to the available services.
    /// </summary>
    /// <param name="services">The services to add to the child DI container.</param>
    /// <returns>The created child DI container.</returns>
    IServiceProvider CreateChildProviderWith(IServiceCollection services);
}
