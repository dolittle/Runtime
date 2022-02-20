// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Resources;

/// <summary>
/// Represents the configuration for resources.
/// </summary>
[Config("resources")]
public class ResourcesConfiguration : Dictionary<Guid, ResourcePerTypeConfiguration>
{
}
