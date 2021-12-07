// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;
namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines a system that knows about event handlers.
/// </summary>
public interface IEventHandlers
{
    /// <summary>
    /// Gets information about all Event Handlers
    /// </summary>
    IEnumerable<EventHandlerInfo> All { get; }

    /// <summary>
    /// Gets the current state of an Event Handler.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId"/>.</param>
    /// <returns>The current state.</returns>
    Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(EventHandlerId eventHandlerId);

    /// <summary>
    /// Registers and starts an event handler.
    /// </summary>
    /// <param name="dispatcher">The actual <see cref="ReverseCallDispatcherType"/>.</param>
    /// <param name="arguments">Connecting arguments.</param>
    /// <param name="cancellationToken">Cancellation token that can cancel the hierarchy.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    Task RegisterAndStart(ReverseCallDispatcherType dispatcher, EventHandlerRegistrationArguments arguments, CancellationToken cancellationToken);

    /// <summary>
    /// Reprocesses all events for an event handler from a <see cref="StreamPosition" /> for a tenant.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId"/> of the identifying the event handler.</param>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    Task<Try<StreamPosition>> ReprocessEventsFrom(EventHandlerId eventHandlerId, TenantId tenant, StreamPosition position);
        
    /// <summary>
    /// Reprocesses all the events for an event handler for all tenants.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId"/> of the identifying the event handler.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> ReprocessAllEvents(EventHandlerId eventHandlerId);
}