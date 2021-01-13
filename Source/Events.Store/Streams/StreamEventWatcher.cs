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
    public class StreamEventWatcher : INotifyOfStreamEvents, IWaitForEventInStream
    {
        readonly ConcurrentDictionary<EventWaiterId, EventWaiter> _waiters = new ConcurrentDictionary<EventWaiterId, EventWaiter>();

        /// <inheritdoc/>
        public void NotifyForEvent(ScopeId scope, StreamId stream, StreamPosition position)
        {
            var waiterId = new EventWaiterId(scope, stream);
            var waiter = _waiters.GetOrAdd(waiterId, id => new EventWaiter(scope, stream));
            waiter.Notify(position);
        }

        /// <inheritdoc/>
        public Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, CancellationToken token)
            => WaitForEvent(scope, stream, position, TimeSpan.FromMinutes(1), token);

        /// <inheritdoc/>
        public async Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token)
        {
            var waiterId = new EventWaiterId(scope, stream);
            using var timeoutSource = new CancellationTokenSource(timeout);
            using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutSource.Token);

            var waiter = _waiters.GetOrAdd(waiterId, id => new EventWaiter(id.Scope, id.Stream));

            await waiter.Wait(position, linkedSource.Token).ConfigureAwait(false);
        }
    }
}
