// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Build.for_BuildTaskPerformers
{
    public class when_performing_with_two_performers_in_the_system
    {
        static BuildTaskPerformers performers;

        static Mock<ICanPerformBuildTask> first_performer;
        static Mock<ICanPerformBuildTask> second_performer;

        Establish context = () =>
        {
            first_performer = new Mock<ICanPerformBuildTask>();
            second_performer = new Mock<ICanPerformBuildTask>();
            var actualInstances = new List<ICanPerformBuildTask>
            {
                first_performer.Object,
                second_performer.Object
            };
            var instances = new Mock<IInstancesOf<ICanPerformBuildTask>>();
            instances.Setup(_ => _.GetEnumerator()).Returns(actualInstances.GetEnumerator());
            performers = new BuildTaskPerformers(instances.Object, Mock.Of<IBuildMessages>());
        };

        Because of = () => performers.Perform();

        It should_call_perform_on_first_performer = () => first_performer.Verify(_ => _.Perform(), Moq.Times.Once());
        It should_call_perform_on_second_performer = () => second_performer.Verify(_ => _.Perform(), Moq.Times.Once());
    }
}