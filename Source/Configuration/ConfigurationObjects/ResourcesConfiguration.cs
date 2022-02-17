// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

/// <summary>
/// Represents the configuration for resources.
/// </summary>
public class ResourcesConfiguration : TenantSpecificConfigurationObject<ResourcePerTypeConfiguration>
{
}
