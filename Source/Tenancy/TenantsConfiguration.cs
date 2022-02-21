// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Represents the configuration of tenants.
/// </summary>
[Configuration("tenants")]
public class TenantsConfiguration : ReadOnlyDictionary<TenantId, TenantConfiguration>
{
    public TenantsConfiguration(IDictionary<TenantId, TenantConfiguration> dictionary)
        : base(dictionary)
    {
    }
}
