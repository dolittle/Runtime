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
        readonly object _notifiedLock = new object();
        readonly object _readWriteLock = new object();
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
        /// Waits for an event to arrive at a given position.
        /// If a position has already been notified about, completes the task immediately.
        /// </summary>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
        public async Task Wait(StreamPosition position, CancellationToken token)
        {
            if (IsAlreadyNotifiedOfPosition(position)) return;
            var tcs = GetOrAddTaskCompletionSourceLocking(position);
            if (IsAlreadyNotifiedOfPosition(position))
            {
                _taskCompletionSources.Remove(position);
                return;
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
            // System.Console.WriteLine($"In Notify for Id {Id} and Position {position} with last notified {_lastNotified}");
            if (NeverNotifiedOrNotNotifiedOfPosition(position))
            {
                lock (_notifiedLock)
                {
                    if (NeverNotifiedOrNotNotifiedOfPosition(position))
                    {
                        // System.Console.WriteLine($"In Notify for Id {Id} and Position {position} with last notified {_lastNotified} settings last notified");
                        _lastNotified = position;
                    }
                }
            }

            RemoveAllAtAndBelowLocking(position);
        }

        void RemoveAllAtAndBelowLocking(StreamPosition position)
        {
            lock (_readWriteLock)
            {
                // System.Console.WriteLine($"There are {_taskCompletionSources.Count} waiters waiting in waiter with hash {GetHashCode()}");
                foreach (var storedPosition in _taskCompletionSources.Keys)
                {
                    // System.Console.WriteLine($"Looping through position {storedPosition.Value} and wanted position {position.Value}");
                    if (storedPosition.Value <= position)
                    {
                        // System.Console.WriteLine($"In Notify for Id {Id} and Position {position} setting position {storedPosition.Value} to completed");
                        _taskCompletionSources[storedPosition].TrySetResult(true);
                    }

                    if (storedPosition == position) break;
                }
            }
        }

        TaskCompletionSource<bool> GetOrAddTaskCompletionSourceLocking(StreamPosition position)
        {
            // System.Console.WriteLine($"Getting or adding {position} to waiter {Id}");
            if (!_taskCompletionSources.TryGetValue(position, out var tcs))
            {
                lock (_readWriteLock)
                {
                    if (!_taskCompletionSources.TryGetValue(position, out tcs))
                    {
                        tcs = new TaskCompletionSource<bool>();
                        _taskCompletionSources.Add(position, tcs);

                        // System.Console.WriteLine($"Added position {position} to waiter {Id} with hash {GetHashCode()}");
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
