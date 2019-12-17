// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concurrency;
using Machine.Specifications;
using Moq;

namespace Dolittle.Tasks.Specs.for_TaskScheduler.given
{
    public class a_task_scheduler
    {
        protected static Mock<IScheduler> scheduler_mock;
        protected static TaskScheduler task_scheduler;

        Establish context = () =>
        {
            scheduler_mock = new Mock<IScheduler>();
            task_scheduler = new TaskScheduler(scheduler_mock.Object);
        };
    }
}
