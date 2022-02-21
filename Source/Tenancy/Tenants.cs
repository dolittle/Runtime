// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.Options;


namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Represents an implementation of <see cref="ITenants"/>.
/// </summary>
[Singleton]
public class Tenants : ITenants
{
    readonly TenantsConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tenants"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="TenantsConfiguration">configuration</see> for tenants.</param>
    public Tenants(IOptions<TenantsConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    /// <inheritdoc/>
    public ObservableCollection<TenantId> All => new(_configuration.Keys);
}
