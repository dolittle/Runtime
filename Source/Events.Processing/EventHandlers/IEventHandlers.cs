// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;
namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Defines a system that knows about event handlers.
    /// </summary>
    public interface IEventHandlers
    {
        /// <summary>
        /// Registers and starts an event handler.
        /// </summary>
        /// <param name="dispatcher">The actual <see cref="ReverseCallDispatcherType"/>.</param>
        /// <param name="arguments">Connecting arguments.</param>
        /// <param name="cancellationToken">Cancellation token that can cancel the hierarchy.</param>
        /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
        Task RegisterAndStart(ReverseCallDispatcherType dispatcher, EventHandlerRegistrationArguments arguments, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the position of an event handler for a tenant.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId"/> of the identifying the event handler.</param>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
        Task<Try<StreamPosition>> SetToPosition(EventHandlerId eventHandlerId, TenantId tenant, StreamPosition position);
        
        /// <summary>
        /// Sets the position of an event handler for all tenant to be the initial <see cref="StreamPosition"/>.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId"/> of the identifying the event handler.</param>
        /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
        Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> SetToInitialForAllTenants(EventHandlerId eventHandlerId);
        
    }
}