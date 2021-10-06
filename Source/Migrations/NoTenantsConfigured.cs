// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.ResourceTypes.Configuration;
namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Exception that gets thrown when <see cref="ResourceConfigurationsByTenant"/> has no tenants configured.
    /// </summary>
    public class NoTenantsConfigured : Exception
    {
        public NoTenantsConfigured()
            : base($"Resource configuration has no tenants configured")
        {}
    }
}