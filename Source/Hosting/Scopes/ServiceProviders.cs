// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Represents an implementation of <see cref="ICreateServiceProvidersForScopedHosts"/>.
/// </summary>
[Singleton]
public class ServiceProviders : ICreateServiceProvidersForScopedHosts
{
    readonly IChildServiceProviderFactory _providerFactory;
    readonly Dictionary<ScopedHostDefinition, IServiceProvider> _providers = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceProviders"/> class.
    /// </summary>
    /// <param name="providerFactory"></param>
    public ServiceProviders(IChildServiceProviderFactory providerFactory)
    {
        _providerFactory = providerFactory;
    }

    /// <inheritdoc />
    public IServiceProvider GetServiceProviderFor(ScopedHostDefinition host)
    {
        if (_providers.TryGetValue(host, out var provider))
        {
            return provider;
        }

        provider = _providerFactory.CreateChildProviderWith(host.ScopedServices);
        _providers.Add(host, provider);
        return provider;
    }
}
