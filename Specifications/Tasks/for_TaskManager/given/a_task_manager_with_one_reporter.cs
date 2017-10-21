using doLittle.Tasks;
using Machine.Specifications;
using Moq;
using doLittle.Execution;
using doLittle.Types;
using doLittle.DependencyInversion;
using System.Collections.Generic;
using System.Linq;

namespace doLittle.Specs.Tasks.for_TaskManager.given
{
    public class a_task_manager_with_one_reporter
    {
        protected static Mock<ITaskRepository> task_repository;
        protected static Mock<ITaskScheduler> task_scheduler;
        protected static Mock<IContainer> container;
        protected static Mock<ITaskStatusReporter> task_status_reporter;
        protected static TaskManager task_manager;

        Establish context = () =>
        {
            task_status_reporter = new Mock<ITaskStatusReporter>();
            task_repository = new Mock<ITaskRepository>();
            task_scheduler = new Mock<ITaskScheduler>();
            container = new Mock<IContainer>();


            task_manager = new TaskManager(task_repository.Object, 
                task_scheduler.Object, 
                new InstancesOf<ITaskStatusReporter>(new[] { task_status_reporter.Object }), 
                container.Object);
        };
    }
}
