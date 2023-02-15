// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IEventHandlers"/>.
/// </summary>
[Singleton]
public class EventHandlers : IEventHandlers
{
    readonly ConcurrentDictionary<EventHandlerId, IEventHandler> _eventHandlers = new();

    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlers"/> class.
    /// </summary>
    /// <param name="metrics">The <see cref="IMetricsCollector"/> to use.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EventHandlers(IMetricsCollector metrics, ILogger logger)
    {
        _metrics = metrics;
        _logger = logger;
    }
        
    /// <inheritdoc />
    public IEnumerable<EventHandlerInfo> All => _eventHandlers.Select(_ => _.Value.Info);

    /// <inheritdoc />
    public async Task<Try<IDictionary<TenantId, IStreamProcessorState>>> CurrentStateFor(EventHandlerId eventHandlerId)
        => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
            ? await eventHandler.GetEventHandlerCurrentState()
            : new EventHandlerNotRegistered(eventHandlerId);
        
    /// <inheritdoc />
    public async Task RegisterAndStart(IEventHandler eventHandler, Func<Failure, CancellationToken, Task> onFailure, CancellationToken cancellationToken)
    {
        var eventHandlerId = eventHandler.Info.Id;
        
        _metrics.IncrementRegistrationsTotal(eventHandler.Info);
        void IncrementFailure() => _metrics.IncrementFailedRegistrationsTotal(eventHandler.Info);
        
        if (!_eventHandlers.TryAdd(eventHandlerId, eventHandler))
        {
            _logger.EventHandlerAlreadyRegistered(eventHandlerId);
            _metrics.IncrementFailedRegistrationsTotal(eventHandler.Info);
            await onFailure(new EventHandlerAlreadyRegistered(eventHandlerId), cancellationToken).ConfigureAwait(false);
            return;
        }
        try
        {
            eventHandler.OnRegistrationFailed += IncrementFailure;
            await eventHandler.RegisterAndStart().ConfigureAwait(false);
        }
        finally
        {
            _eventHandlers.Remove(eventHandlerId, out _);
            eventHandler.OnRegistrationFailed -= IncrementFailure;
        }
    }

    /// <inheritdoc />
    public Task<Try<ProcessingPosition>> ReprocessEventsFrom(EventHandlerId eventHandlerId, TenantId tenant, ProcessingPosition position)
        => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
            ? eventHandler.ReprocessEventsFrom(tenant, position)
            : Task.FromResult<Try<ProcessingPosition>>(new EventHandlerNotRegistered(eventHandlerId));

    /// <inheritdoc />
    public Task<Try<IDictionary<TenantId, Try<ProcessingPosition>>>> ReprocessAllEvents(EventHandlerId eventHandlerId)
        => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
            ? eventHandler.ReprocessAllEvents()
            : Task.FromResult<Try<IDictionary<TenantId, Try<ProcessingPosition>>>>(new EventHandlerNotRegistered(eventHandlerId));
}
