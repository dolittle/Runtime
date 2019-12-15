// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA2008

namespace Dolittle.Concurrency
{
    /// <summary>
    /// Represents a <see cref="IScheduler"/>.
    /// </summary>
    public class Scheduler : IScheduler
    {
        readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokens = new Dictionary<Guid, CancellationTokenSource>();

        /// <inheritdoc/>
        public Guid Start(Action action, Action actionDone = null)
        {
            var id = Guid.NewGuid();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            _cancellationTokens[id] = cancellationTokenSource;

            var task = Task.Run(action, cancellationToken);

            task.ContinueWith(_ =>
            {
                _cancellationTokens.Remove(id);
                actionDone?.Invoke();
            });

            return id;
        }

        /// <inheritdoc/>
        public Guid Start<T>(Action<T> action, T objectState, Action<T> actionDone = null)
        {
            var id = Guid.NewGuid();
            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokens[id] = cancellationTokenSource;

            var task = Task.Run(() => action(objectState));

            task.ContinueWith(_ =>
            {
                _cancellationTokens.Remove(id);
                actionDone?.Invoke(objectState);
            });

            return id;
        }

        /// <inheritdoc/>
        public void Stop(Guid id)
        {
            if (_cancellationTokens.ContainsKey(id))
                _cancellationTokens[id].Cancel();
        }
    }
}
