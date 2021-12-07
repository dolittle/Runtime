// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamEventWatcher" /> and <see cref="IWaitForEventInStream" />.
/// </summary>
[SingletonPerTenant]
public class StreamEventWatcher : IStreamEventWatcher
{
    readonly ConcurrentDictionary<EventWaiterId, EventWaiter> _waiters = new();
    readonly ConcurrentDictionary<EventWaiterId, EventWaiter> _publicWaiters = new();
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamEventWatcher"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public StreamEventWatcher(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void NotifyForEvent(ScopeId scope, StreamId stream, StreamPosition position)
        => NotifyForEvent(scope, stream, position, false);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, CancellationToken token)
        => WaitForEventAtPositionWithTimeout(scope, stream, position, TimeSpan.FromMinutes(1), false, token);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token)
        => WaitForEventAtPositionWithTimeout(scope, stream, position, timeout, false, token);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, TimeSpan timeout, CancellationToken token)
        => WaitForEventAppendedWithTimeout(scope, stream, timeout, false, token);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, CancellationToken token)
        => WaitForEventAppendedWithTimeout(scope, stream, TimeSpan.FromMinutes(1), false, token);

    /// <inheritdoc/>
    public void NotifyForEvent(StreamId stream, StreamPosition position)
        => NotifyForEvent(ScopeId.Default, stream, position, true);

    /// <inheritdoc/>
    public Task WaitForEvent(StreamId stream, StreamPosition position, CancellationToken token)
        => WaitForEventAtPositionWithTimeout(ScopeId.Default, stream, position, TimeSpan.FromMinutes(1), true, token);

    /// <inheritdoc/>
    public Task WaitForEvent(StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token)
        => WaitForEventAtPositionWithTimeout(ScopeId.Default, stream, position, timeout, true, token);

    static EventWaiter CreateNewWaiter(EventWaiterId id)
        => new(id.Scope, id.Stream);

    Task WaitForEventAtPositionWithTimeout(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, bool isPublic, CancellationToken token)
        => WaitForWithTimeout(
            scope,
            stream,
            timeout,
            isPublic,
            (waiter, token) => {
                _logger.WaitForEventAtPosition(position, isPublic, stream, scope);
                return waiter.Wait(position, token);
            },
            token);

    Task WaitForEventAppendedWithTimeout(ScopeId scope, StreamId stream, TimeSpan timeout, bool isPublic, CancellationToken token)
        => WaitForWithTimeout(
            scope,
            stream,
            timeout,
            isPublic,
            (waiter, token) => {
                _logger.WaitForEventAppended(isPublic, stream, scope);
                return waiter.Wait(token);
            },
            token);

    async Task WaitForWithTimeout(ScopeId scope, StreamId stream, TimeSpan timeout, bool isPublic, Func<EventWaiter, CancellationToken, Task> waitCallback, CancellationToken token)
    {
        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeoutTokenSource.CancelAfter(timeout);

        var waiterId = new EventWaiterId(scope, stream);
        var waiter = isPublic
            ? _publicWaiters.GetOrAdd(waiterId, CreateNewWaiter)
            : _waiters.GetOrAdd(waiterId, CreateNewWaiter);

        try
        {
            await waitCallback(waiter, timeoutTokenSource.Token).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            _logger.WaitingTimedOut(waiter.Id);
        }
    }

    void NotifyForEvent(ScopeId scope, StreamId stream, StreamPosition position, bool isPublic)
    {
        _logger.WaiterNotifyForEvent(position, isPublic, stream, scope);
        var waiterId = new EventWaiterId(scope, stream);

        var waiter = isPublic
            ? _publicWaiters.GetOrAdd(waiterId, CreateNewWaiter)
            : _waiters.GetOrAdd(waiterId, CreateNewWaiter);
        waiter.Notify(position);
    }
}