// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Concurrency
{
    /// <summary>
    /// Defines a scheduler for scheduling operations.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Start an <see cref="Action"/> on a seperate thread.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to perform.</param>
        /// <param name="actionDone">Optional <see cref="Action"/> to call when it is done.</param>
        /// <returns>Identifier of the task.</returns>
        Guid Start(Action action, Action actionDone = null);

        /// <summary>
        /// Start an <see cref="Action{T}"/> on a seperate thread with state passed along to the <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of state.</typeparam>
        /// <param name="action"><see cref="Action{T}"/> to perform.</param>
        /// <param name="objectState">State to pass along to the action.</param>
        /// <param name="actionDone">Optional <see cref="Action{T}"/> to call when it is done.</param>
        /// <returns>Identifier for the task.</returns>
        Guid Start<T>(Action<T> action, T objectState, Action<T> actionDone = null);

        /// <summary>
        /// Stop a scheduled <see cref="Action"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> if the task to stop.</param>
        void Stop(Guid id);
    }
}
