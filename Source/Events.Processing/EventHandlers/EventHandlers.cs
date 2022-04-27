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
    
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlers"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EventHandlers(ILogger logger)
    {
        _logger = logger;
    }
        
    /// <inheritdoc />
    public IEnumerable<EventHandlerInfo> All => _eventHandlers.Select(_ => _.Value.Info);

    /// <inheritdoc />
    public Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(EventHandlerId eventHandlerId)
        => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
            ? eventHandler.GetEventHandlerCurrentState()
            : new EventHandlerNotRegistered(eventHandlerId);
        
    /// <inheritdoc />
    public async Task RegisterAndStart(IEventHandler eventHandler, Func<Failure, CancellationToken, Task> onFailure, CancellationToken cancellationToken)
    {
        var eventHandlerId = eventHandler.Info.Id;
        if (!_eventHandlers.TryAdd(eventHandlerId, eventHandler))
        {
            Log.EventHandlerAlreadyRegistered(_logger, eventHandlerId);
            await onFailure(new EventHandlerAlreadyRegistered(eventHandlerId), cancellationToken).ConfigureAwait(false);
            return;
        }
        try
        {
            await eventHandler.RegisterAndStart().ConfigureAwait(false);
        }
        finally
        {
            _eventHandlers.Remove(eventHandlerId, out _);
        }
    }

    /// <inheritdoc />
    public Task<Try<StreamPosition>> ReprocessEventsFrom(EventHandlerId eventHandlerId, TenantId tenant, StreamPosition position)
        => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
            ? eventHandler.ReprocessEventsFrom(tenant, position)
            : Task.FromResult<Try<StreamPosition>>(new EventHandlerNotRegistered(eventHandlerId));

    /// <inheritdoc />
    public Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> ReprocessAllEvents(EventHandlerId eventHandlerId)
        => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
            ? eventHandler.ReprocessAllEvents()
            : Task.FromResult<Try<IDictionary<TenantId, Try<StreamPosition>>>>(new EventHandlerNotRegistered(eventHandlerId));
}
