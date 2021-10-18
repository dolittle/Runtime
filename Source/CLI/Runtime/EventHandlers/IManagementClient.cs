// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;

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
    }
}