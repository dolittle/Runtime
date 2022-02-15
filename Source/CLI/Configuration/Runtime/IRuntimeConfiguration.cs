// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Resources;

namespace Dolittle.Runtime.CLI.Configuration.Runtime;

/// <summary>
/// Defines the configuration for a Runtime.
/// </summary>
public interface IRuntimeConfiguration
{
    /// <summary>
    /// Gets the resource configurations for all tenants.
    /// </summary>
    ResourceConfigurationsByTenant Resources { get; }
}
