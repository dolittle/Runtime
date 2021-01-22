// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Defines the basics for an async policy.
    /// </summary>
    public interface IAsyncPolicy
    {
        /// <summary>
        /// Execute an async action within the policy.
        /// </summary>
        /// <param name="action"><see cref="Func{Task}"/> to execute.</param>
        /// <returns>The Task.</returns>
        [DebuggerStepThrough]
        Task Execute(Func<Task> action);

        /// <summary>
        /// Execute an async action within the policy.
        /// </summary>
        /// <param name="action"><see cref="Func{CancellationToken, Task}"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The Task.</returns>
        [DebuggerStepThrough]
        Task Execute(Func<CancellationToken, Task> action, CancellationToken cancellationToken);

        /// <summary>
        /// Execute an async action within the policy.
        /// </summary>
        /// <param name="action"><see cref="Func{CancellationToken, Task}"/> to execute.</param>
        /// <param name="continueOnCapturedContext">Whether to continue on a captured synchronization context.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The Task.</returns>
        [DebuggerStepThrough]
        Task Execute(Func<CancellationToken, Task> action, bool continueOnCapturedContext, CancellationToken cancellationToken);

        /// <summary>
        /// /// Executes an action within the policy and returns the result from the action.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="action"><see cref="Func{Task}"/> to call.</param>
        /// <returns>Result from the action.</returns>
        [DebuggerStepThrough]
        Task<TResult> Execute<TResult>(Func<Task<TResult>> action);

        /// <summary>
        /// Executes an action within the policy and returns the result from the action.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="action"><see cref="Func{CancellationToken, Task}"/> to call.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>Result from the action.</returns>
        [DebuggerStepThrough]
        Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken);

        /// <summary>
        /// Executes an action within the policy and returns the result from the action.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="action"><see cref="Func{CancellationToken, Task}"/> to call.</param>
        /// <param name="continueOnCapturedContext">Whether to continue on a captured synchronization context.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>Result from the action.</returns>
        [DebuggerStepThrough]
        Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> action, bool continueOnCapturedContext, CancellationToken cancellationToken);
    }
}