using Machine.Specifications;
using System;
using doLittle.Tasks;

namespace doLittle.Specs.Tasks.for_TaskManager
{
    public class when_starting_and_it_gets_done : given.a_task_manager_with_one_reporter
    {
        static TaskId task_id = TaskId.New();
        static OurTask task;

        Establish context = () => {
            task = new OurTask
            {
                CurrentOperation = 1
            };
            task_repository.Setup(t => t.Load(Moq.It.IsAny<TaskId>())).Returns(task);
            container.Setup(c => c.Get<OurTask>()).Returns(task);
            task_scheduler.Setup(c => c.Start(task, Moq.It.IsAny<Action<Task>>())).Callback((Task t, Action<Task> a) => a(t));
        };

        Because of = () => task_manager.Start<OurTask>();

        It should_end_task = () => task.EndCalled.ShouldBeTrue();
        It should_call_the_status_reporter = () => task_status_reporter.Verify(t => t.Stopped(task), Moq.Times.Once());
        It should_delete_task = () => task_repository.Verify(t => t.Delete(task), Moq.Times.Once());
    }
}
