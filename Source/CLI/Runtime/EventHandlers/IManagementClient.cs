// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Defines the EventHandlers management client.
/// </summary>
public interface IManagementClient
{
    /// <summary>
    /// Reprocesses events from a position for an Event Handler for a tenant.
    /// </summary>
    /// <param name="eventHandler">The Event Handler identifier.</param>
    /// <param name="tenant">The tenant to reprocess events for.</param>
    /// <param name="position">The position to start reprocessing from.</param>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ReprocessEventsFrom(EventHandlerId eventHandler, TenantId tenant, StreamPosition position, MicroserviceAddress runtime);
        
    /// <summary>
    /// Reprocesses all events an Event Handler for all tenants.
    /// </summary>
    /// <param name="eventHandler">The Event Handler identifier.</param>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ReprocessAllEvents(EventHandlerId eventHandler, MicroserviceAddress runtime);
        
    /// <summary>
    /// Get the <see cref="EventHandlerStatus"/> of all registered Event Handlers.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="tenant">The Tenant to get Stream Processor states for, or null to get all.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="EventHandlerStatus"/> of all registered Projections.</returns>
    Task<IEnumerable<EventHandlerStatus>> GetAll(MicroserviceAddress runtime, TenantId? tenant = null);

    /// <summary>
    /// Get the <see cref="EventHandlerStatus"/> of a registered Event Handler by <see cref="EventHandlerId"/>.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="eventHandler">The Event Handler identifier.</param>
    /// <param name="tenant">The Tenant to get Stream Processor states for, or null to get all.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="Try"/> containing the <see cref="EventHandlerStatus"/>-</returns>
    Task<Try<EventHandlerStatus>> Get(MicroserviceAddress runtime, EventHandlerId eventHandler, TenantId? tenant);

}
