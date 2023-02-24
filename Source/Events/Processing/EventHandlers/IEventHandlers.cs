// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines a system that knows about event handlers.
/// </summary>
public interface IEventHandlers
{
    /// <summary>
    /// Gets information about all currently registered Event Handlers
    /// </summary>
    IEnumerable<EventHandlerInfo> All { get; }

    /// <summary>
    /// Gets the current state of an Event Handler for all tenants.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId"/>.</param>
    /// <returns>The current states of the Event Handler.</returns>
    Task<Try<IDictionary<TenantId, IStreamProcessorState>>> CurrentStateFor(EventHandlerId eventHandlerId);

    /// <summary>
    /// Registers and starts an Event Handler for all tenants.
    /// </summary>
    /// <param name="eventHandler">The event handler to to register and start.</param>
    /// <param name="onFailure">The <see cref="Func{T}"/> callback for failures.</param>
    /// <param name="cancellationToken">Cancellation token that can cancel the hierarchy.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous processing operation.</returns>
    Task RegisterAndStart(IEventHandler eventHandler, Func<Failure, CancellationToken, Task> onFailure, CancellationToken cancellationToken);

    /// <summary>
    /// Reprocesses all events for an event handler from a <see cref="StreamPosition" /> for a tenant.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId"/> of the identifying the event handler.</param>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    Task<Try<ProcessingPosition>> ReprocessEventsFrom(EventHandlerId eventHandlerId, TenantId tenant, ProcessingPosition position);
        
    /// <summary>
    /// Reprocesses all the events for an event handler for all tenants.
    /// </summary>
    /// <param name="eventHandlerId">The <see cref="EventHandlerId"/> of the identifying the event handler.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    Task<Try<IDictionary<TenantId, Try<ProcessingPosition>>>> ReprocessAllEvents(EventHandlerId eventHandlerId);
    
}
