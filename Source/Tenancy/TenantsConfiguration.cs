// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Represents the configuration for tenants.
/// </summary>
public class TenantsConfiguration :
    ReadOnlyDictionary<Guid, TenantConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantsConfiguration"/> class.
    /// </summary>
    /// <param name="tenants"><see cref="IDictionary{TKey, TValue}"/> with tenants and their configuration.</param>
    public TenantsConfiguration(IDictionary<Guid, TenantConfiguration> tenants)
        : base(tenants)
    {
    }
}
