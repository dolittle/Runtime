// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

public interface IEventHandler : IDisposable
{
    /// <summary>
    /// Gets the <see cref="EventHandlerInfo"/> for the <see cref="EventHandler"/>.
    /// </summary>
    EventHandlerInfo Info { get; }

    /// <summary>
    /// Gets the current state of the Event Handler. 
    /// </summary>
    /// <returns>The  <see cref="IStreamProcessorState"/> per <see cref="TenantId"/>.</returns>
    Task<Try<IDictionary<TenantId, IStreamProcessorState>>> GetEventHandlerCurrentState();

    /// <summary>
    /// Reprocesses all events from a <see cref="StreamPosition" /> for a tenant.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    Task<Try<ProcessingPosition>> ReprocessEventsFrom(TenantId tenant, ProcessingPosition position);

    /// <summary>
    /// Reprocesses all the events for all tenants.
    /// </summary>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    Task<Try<IDictionary<TenantId, Try<ProcessingPosition>>>> ReprocessAllEvents();

    /// <summary>
    /// Register and start the event handler for filtering and processing.
    /// </summary>
    /// <returns>Async <see cref="Task"/>.</returns>
    Task RegisterAndStart();

    /// <summary>
    /// Event that occurs if the EventHandler registration fails.
    /// </summary>
    // event EventHandlerRegistrationFailed OnRegistrationFailed;
}
