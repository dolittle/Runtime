// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
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
        readonly object _lock = new();
        readonly SortedList<StreamPosition, TaskCompletionSource<bool>> _taskCompletionSources;
        StreamPosition _lastNotified;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventWaiter"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public EventWaiter(ScopeId scope, StreamId stream)
        {
            Id = new EventWaiterId(scope, stream);
            _taskCompletionSources = new SortedList<StreamPosition, TaskCompletionSource<bool>>(
                Comparer<StreamPosition>.Create((a, b) => a.Value.CompareTo(b.Value)));
        }

        /// <summary>
        /// Gets the <see cref="EventWaiterId" />.
        /// </summary>
        public EventWaiterId Id { get; }

        /// <summary>
        /// Waits for an event to be appended to the stream.
        /// </summary>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
        public async Task Wait(CancellationToken token)
        {
            TaskCompletionSource<bool> tcs;
            lock (_lock)
            {
                var nextExpectedPosition = _lastNotified == null ? StreamPosition.Start : (StreamPosition)(_lastNotified + 1);
                tcs = GetOrAddTaskCompletionSourceLocking(nextExpectedPosition);
            }
            await tcs.Task.WaitAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits for an event to arrive at a given position.
        /// </summary>
        /// <remarks>
        /// If a position has already been notified about, completes the task immediately.
        /// </remarks>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
        public async Task Wait(StreamPosition position, CancellationToken token)
        {
            if (IsAlreadyNotifiedOfPosition(position)) return;
            TaskCompletionSource<bool> tcs;
            lock (_lock)
            {
                if (IsAlreadyNotifiedOfPosition(position)) return;
                tcs = GetOrAddTaskCompletionSourceLocking(position);
            }
            await tcs.Task.WaitAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Notifies that an event has been written in a <see cref="StreamPosition" /> in a stream.
        /// </summary>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        public void Notify(StreamPosition position)
        {
            if (NeverNotifiedOrNotNotifiedOfPosition(position))
            {
                var shouldUpdate = false;
                lock (_lock)
                {
                    if (NeverNotifiedOrNotNotifiedOfPosition(position))
                    {
                        _lastNotified = position;
                        shouldUpdate = true;
                    }
                }
                if (shouldUpdate) RemoveAllAtAndBelowLocking(position);
            }
        }

        void RemoveAllAtAndBelowLocking(StreamPosition position)
        {
            lock (_lock)
            {
                var keys = _taskCompletionSources.Keys.ToArray();
                foreach (var storedPosition in keys)
                {
                    if (storedPosition.Value <= position.Value)
                    {
                        _taskCompletionSources[storedPosition].TrySetResult(true);
                        _taskCompletionSources.Remove(storedPosition);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        TaskCompletionSource<bool> GetOrAddTaskCompletionSourceLocking(StreamPosition position)
        {
            if (!_taskCompletionSources.TryGetValue(position, out var tcs))
            {
                lock (_lock)
                {
                    if (!_taskCompletionSources.TryGetValue(position, out tcs))
                    {
                        tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                        _taskCompletionSources.Add(position, tcs);
                    }
                }
            }

            return tcs;
        }

        bool NeverNotifiedOrNotNotifiedOfPosition(StreamPosition position)
            => _lastNotified == null || _lastNotified.Value < position.Value;

        bool IsAlreadyNotifiedOfPosition(StreamPosition position)
            => _lastNotified != null && _lastNotified.Value >= position.Value;
    }
}
