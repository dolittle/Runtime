// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Exception that gets thrown when a tenant-specific configuration is attempted resolved from the root container.
/// </summary>
public class CannotCreateTenantSpecificConfigurationFromRootContainer : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotCreateTenantSpecificConfigurationFromRootContainer"/> class.
    /// </summary>
    public CannotCreateTenantSpecificConfigurationFromRootContainer()
        : base("Cannot create tenant-specific configuration from the root container")
    {
    }
}
