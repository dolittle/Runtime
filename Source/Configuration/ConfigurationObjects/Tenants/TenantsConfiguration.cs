// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Tenants;

/// <summary>
/// Represents the configuration for tenants.
/// </summary>
[Config("tenants")]
public class TenantsConfiguration : Dictionary<Guid, object>
{
}
