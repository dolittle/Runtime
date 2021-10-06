// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.ResourceTypes.Configuration;
namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Exception that gets thrown when <see cref="ResourceConfigurationsByTenant"/> does not have the an event store configured for the given tenant.
    /// </summary>
    public class EventStoreNotConfiguredForTenant : Exception
    {
        public EventStoreNotConfiguredForTenant(TenantId tenant)
            : base($"Resource configuration does not have an event store configured for tenant {tenant.Value}")
        {}
    }
}