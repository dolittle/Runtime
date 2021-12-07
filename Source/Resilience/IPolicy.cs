// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Defines the basics for a policy.
/// </summary>
public interface IPolicy
{
    /// <summary>
    /// Execute an action within the policy.
    /// </summary>
    /// <param name="action"><see cref="Action"/> to execute.</param>
    [DebuggerStepThrough]
    void Execute(Action action);

    /// <summary>
    /// Executes an action within the policy and returns the result from the action.
    /// </summary>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <param name="action"><see cref="Func{TResult}"/> to call.</param>
    /// <returns>Result from the action.</returns>
    [DebuggerStepThrough]
    TResult Execute<TResult>(Func<TResult> action);
}