// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents a waiter for an event.
    /// </summary>
    /// <remarks>
    /// When the memory data is wiped but there are notified events in a stream, if doing a wait call for an earlier position it would never be done waiting.
    /// </remarks>
    public class EventWaiter
    {
        readonly IDictionary<StreamPosition, TaskCompletionSource<bool>> _taskCompletionSources;
        StreamPosition _lastNotified;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventWaiter"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public EventWaiter(ScopeId scope, StreamId stream)
        {
            Id = new EventWaiterId(scope, stream);
            _taskCompletionSources = new Dictionary<StreamPosition, TaskCompletionSource<bool>>();
        }

        /// <summary>
        /// Gets the <see cref="EventWaiterId" />.
        /// </summary>
        public EventWaiterId Id { get; }

        /// <summary>
        /// Waits for an event to arrive at a given position.
        /// If a position has already been notified about, completes the task immediately.
        /// </summary>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
        public async Task Wait(StreamPosition position, CancellationToken token)
        {
            if (IsAlreadyNotified(position)) return;

            if (!_taskCompletionSources.TryGetValue(position, out var tcs))
            {
                tcs = new TaskCompletionSource<bool>();
                _taskCompletionSources.Add(position, tcs);
            }

            await tcs.Task.WaitAsync(token).ConfigureAwait(false);
            _taskCompletionSources.Remove(position);
        }

        /// <summary>
        /// Notifies that an event has been written in a <see cref="StreamPosition" /> in a stream.
        /// </summary>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        public void Notify(StreamPosition position)
        {
            SetLastNotified(position);
            if (!_taskCompletionSources.TryGetValue(position, out var tcs))
            {
                tcs = new TaskCompletionSource<bool>();
                _taskCompletionSources.Add(position, tcs);
            }

            tcs.SetResult(true);
        }

        void SetLastNotified(StreamPosition position)
            => _lastNotified = IsAlreadyNotified(position)
                ? _lastNotified : position;

        bool IsAlreadyNotified(StreamPosition position)
            => _lastNotified != null && _lastNotified.Value > position.Value;
    }
}
