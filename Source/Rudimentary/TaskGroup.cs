// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Represents a group of <see cref="Task"/> that can be awaited and cancelled together when one completes or fails.
/// </summary>
public class TaskGroup
{
    readonly Task[] _tasks;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskGroup"/> class.
    /// </summary>
    /// <param name="tasks">The tasks to group.</param>
    public TaskGroup(IEnumerable<Task> tasks)
    {
        _tasks = tasks.ToArray();
    }

    /// <summary>
    /// Event that occurs when the first task in the group completes.
    /// </summary>
    public event Action<Task> OnFirstTaskCompleted;
    
    /// <summary>
    /// Event that occurs when the first task in the group fails with an <see cref="Exception"/>.
    /// </summary>
    public event Action<Task, Exception> OnFirstTaskFailure;
    
    /// <summary>
    /// Event that occurs when all of the tasks in the group have completed.
    /// </summary>
    public event Action OnAllTasksCompleted;
    
    /// <summary>
    /// Event that occurs when any other task in the group other than the first fails with an <see cref="Exception"/>.
    /// </summary>
    public event Action<Task, Exception> OnOtherTaskFailures; 

    /// <summary>
    /// Waits for the first task to complete, then cancels the provided token source, and waits for all to complete.
    /// Throws the exception from the first task to complete, if it failed.
    /// </summary>
    /// <param name="cancellationTokenSource">The cancellation token source to cancel when the first task completes.</param>
    public async Task WaitForAllCancellingOnFirst(CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            await Task.WhenAny(_tasks).ConfigureAwait(false);
        }
        finally
        {
            var firstFailure = GetFirstFailureAndInvokeFirstCompleted();
            
            cancellationTokenSource.Cancel();

            try
            {
                await Task.WhenAll(_tasks).ConfigureAwait(false);
            }
            catch
            {
            }
            
            OnAllTasksCompleted?.Invoke();
            InvokeOtherTaskFailures(firstFailure);

            if (firstFailure != default)
            {
                ExceptionDispatchInfo.Capture(firstFailure).Throw();
            }
        }
    }

    Exception GetFirstFailureAndInvokeFirstCompleted()
    {
        foreach (var task in _tasks)
        {
            if (!task.IsCompleted)
            {
                continue;
            }
            
            OnFirstTaskCompleted?.Invoke(task);
            if (task.Exception != default)
            {
                var failure = task.Exception.GetInnerMostException();
                OnFirstTaskFailure?.Invoke(task, failure);
                return failure;
            }
            break;
        }

        return default;
    }

    void InvokeOtherTaskFailures(Exception firstFailure)
    {
        foreach (var task in _tasks)
        {
            if (task.Exception == default)
            {
                continue;
            }
            
            var failure = task.Exception.GetInnerMostException();
            if (failure != firstFailure)
            {
                OnOtherTaskFailures?.Invoke(task, failure);
            }
        }
    }
}
