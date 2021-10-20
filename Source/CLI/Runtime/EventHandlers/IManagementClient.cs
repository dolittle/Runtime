// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
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
        /// Gets all running Event Handlers or for a specific Tenant if specified.
        /// </summary>
        /// <param name="runtime">The address of the Runtime to connect to.</param>
        /// <param name="tenant">The Tenant to get Event Handlers for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<EventHandlerStatus>> GetAll(MicroserviceAddress runtime, TenantId tenant = null);

        /// <summary>
        /// Gets the running Event Handler with the given identifier and for a specific Tenant if specified.
        /// </summary>
        /// <param name="runtime">The address of the Runtime to connect to.</param>
        /// <param name="eventHandler">The Event Handler identifier.</param>
        /// <param name="tenant">The Tenant to get Event Handlers for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Try<EventHandlerStatus>> Get(MicroserviceAddress runtime, EventHandlerId eventHandler, TenantId tenant = null);
    }
}