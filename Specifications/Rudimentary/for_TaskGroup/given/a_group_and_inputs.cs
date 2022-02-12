// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Rudimentary.for_TaskGroup.given;

public class a_group_and_inputs
{
    static Task[] tasks;
    protected static Task first_task
    {
        get => tasks[0];
        set => tasks[0] = value;
    }
    protected static Task second_task
    {
        get => tasks[1];
        set => tasks[1] = value;
    }
    
    protected static CancellationTokenSource cancellation_token_source;

    protected static Mock<Action<Task>> on_first_task_completed;
    protected static Mock<Action<Task, Exception>> on_first_task_failure;
    protected static Mock<Action> on_all_tasks_completed;
    protected static Mock<Action<Task, Exception>> on_other_task_failures;

    protected static TaskGroup task_group;

    Establish context = () =>
    {
        cancellation_token_source = new CancellationTokenSource();

        tasks = new Task[2];
        task_group = new TaskGroup(tasks);

        on_first_task_completed = new Mock<Action<Task>>();
        on_first_task_failure = new Mock<Action<Task, Exception>>();
        on_all_tasks_completed = new Mock<Action>();
        on_other_task_failures = new Mock<Action<Task, Exception>>();

        task_group.OnFirstTaskCompleted += on_first_task_completed.Object;
        task_group.OnFirstTaskFailure += on_first_task_failure.Object;
        task_group.OnAllTasksCompleted += on_all_tasks_completed.Object;
        task_group.OnOtherTaskFailures += on_other_task_failures.Object;
    };

    protected static Exception exception;
    
    Because of = () => exception = Catch.Exception(() => task_group.WaitForAllCancellingOnFirst(cancellation_token_source).GetAwaiter().GetResult());
}