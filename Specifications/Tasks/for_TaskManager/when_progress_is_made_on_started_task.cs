// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Tasks.Specs.for_TaskManager
{
    public class when_progress_is_made_on_started_task : given.a_task_manager_with_one_reporter
    {
        static OurTask task;
        static bool save_called;

        Establish context = () =>
        {
            task = new OurTask
            {
                CurrentOperation = 1
            };
            container.Setup(c => c.Get<OurTask>()).Returns(task);
            task_manager.Start<OurTask>();
            task_repository.Setup(c => c.Save(task)).Callback(() => save_called = true);
        };

        Because of = () => task.Progress();
        It should_save_task = () => save_called.ShouldBeTrue();
        It should_call_the_status_reporter = () => task_status_reporter.Verify(t => t.StateChanged(task), Moq.Times.Once());
    }
}
