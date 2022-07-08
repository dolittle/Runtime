// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="IChildServiceProviderFactory"/> that uses Autofac child lifetime scopes.
/// </summary>
[Singleton]
public class ChildServiceProviderFactory : IChildServiceProviderFactory
{
    readonly ILifetimeScope _globalContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChildServiceProviderFactory"/> class.
    /// </summary>
    /// <param name="globalContainer">The global Autofac container instance.</param>
    public ChildServiceProviderFactory(ILifetimeScope globalContainer)
    {
        _globalContainer = globalContainer;
    }

    /// <inheritdoc />
    public IServiceProvider CreateChildProviderWith(IServiceCollection services)
    {
        var childContainer = _globalContainer.BeginLifetimeScope(builder =>
        {
            builder.Populate(services);
        });
        return new AutofacServiceProvider(childContainer);
    }
}
