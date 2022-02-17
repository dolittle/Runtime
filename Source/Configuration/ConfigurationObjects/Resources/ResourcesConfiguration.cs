// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Resources;

/// <summary>
/// Represents the configuration for resources.
/// </summary>
[Name("resources")]
public class ResourcesConfiguration : TenantSpecificConfigurationObject<ResourcePerTypeConfiguration>
{
}
