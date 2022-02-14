// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Rudimentary.for_TaskGroup;

public class when_second_task_fails : given.a_group_and_inputs
{
    static Exception failure;
    Establish context = () =>
    {
        failure = new Exception();
        first_task = Task.Delay(20);
        second_task = Task.Delay(10).ContinueWith(_ => throw failure);
    };

    It should_throw_the_first_exception = () => exception.ShouldBeTheSameAs(failure);
    It should_call_first_completed_with_first_task = () => on_first_task_completed.Verify(_ => _(second_task), Times.Once);
    It should_call_all_completed = () => on_all_tasks_completed.Verify(_ => _(), Times.Once);
    It should_call_first_failure = () => on_first_task_failure.Verify(_ => _(second_task, failure), Times.Once);
    It should_not_call_other_failures = () => on_other_task_failures.VerifyNoOtherCalls();
}