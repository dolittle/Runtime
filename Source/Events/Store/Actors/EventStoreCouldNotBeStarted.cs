// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Collections.Generic;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Exception that gets thrown when the event store could not be started.
/// </summary>
public class EventStoreCouldNotBeStarted : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreCouldNotBeStarted"/> class.
    /// </summary>
    /// <param name="tenant">The tenant the event store is started for.</param>
    /// <param name="error">The error that caused it.</param>
    /// <param name="configuredTenants">The currently configured tenants.</param>
    public EventStoreCouldNotBeStarted(TenantId tenant, Exception error, IEnumerable<TenantId> configuredTenants)
        : base($"Failed to start Event Store virtual actor for tenant {tenant}. This might be caused by a missing or bad configuration. " +
            $"Please check that 'resources' and 'tenants' are correctly configured. " +
            $"These are the tenants that the Runtime knows about: {string.Join(", ", configuredTenants)}", error)
    {
    }
}
