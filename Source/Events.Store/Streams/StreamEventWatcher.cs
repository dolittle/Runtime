// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="INotifyOfStreamEvents" /> and <see cref="IWaitForEventInStream" />.
    /// </summary>
    [SingletonPerTenant]
    public class StreamEventWatcher : INotifyOfStreamEvents, IWaitForEventInStream, INotifyOfPublicStreamEvents, IWaitForEventInPublicStream
    {
        readonly ConcurrentDictionary<EventWaiterId, EventWaiter> _waiters = new ConcurrentDictionary<EventWaiterId, EventWaiter>();
        readonly ConcurrentDictionary<EventWaiterId, EventWaiter> _publicWaiters = new ConcurrentDictionary<EventWaiterId, EventWaiter>();

        /// <inheritdoc/>
        void INotifyOfStreamEvents.NotifyForEvent(ScopeId scope, StreamId stream, StreamPosition position)
            => NotifyForEvent(scope, stream, position, false);

        /// <inheritdoc/>
        Task IWaitForEventInStream.WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, CancellationToken token)
            => WaitForWaiter(scope, stream, position, TimeSpan.FromMinutes(1), false, token);

        /// <inheritdoc/>
        Task IWaitForEventInStream.WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token)
            => WaitForWaiter(scope, stream, position, timeout, false, token);

        /// <inheritdoc/>
        void INotifyOfPublicStreamEvents.NotifyForEvent(StreamId stream, StreamPosition position)
            => NotifyForEvent(ScopeId.Default, stream, position, true);

        /// <inheritdoc/>
        Task IWaitForEventInPublicStream.WaitForEvent(StreamId stream, StreamPosition position, CancellationToken token)
            => WaitForWaiter(ScopeId.Default, stream, position, TimeSpan.FromMinutes(1), true, token);

        /// <inheritdoc/>
        Task IWaitForEventInPublicStream.WaitForEvent(StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token)
            => WaitForWaiter(ScopeId.Default, stream, position, timeout, true, token);

        async Task WaitForWaiter(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, bool isPublic, CancellationToken token)
        {
            var waiterId = new EventWaiterId(scope, stream);
            using var timeoutSource = new CancellationTokenSource(timeout);
            using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutSource.Token);

            var newWaiter = new EventWaiter(scope, stream);
            var waiter = isPublic
                ? _publicWaiters.GetOrAdd(waiterId, id => newWaiter)
                : _waiters.GetOrAdd(waiterId, id => newWaiter);

            await waiter.Wait(position, linkedSource.Token).ConfigureAwait(false);
        }

        void NotifyForEvent(ScopeId scope, StreamId stream, StreamPosition position, bool isPublic)
        {
            var waiterId = new EventWaiterId(ScopeId.Default, stream);
            var newWaiter = new EventWaiter(scope, stream);
            var waiter = isPublic
                ? _publicWaiters.GetOrAdd(waiterId, id => newWaiter)
                : _waiters.GetOrAdd(waiterId, id => newWaiter);
            waiter.Notify(position);
        }
    }
}
