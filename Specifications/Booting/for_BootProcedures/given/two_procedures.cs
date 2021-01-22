// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Booting.Specs.for_BootProcedures.given
{
    public class two_procedures : all_dependencies
    {
        protected static Mock<ICanPerformBootProcedure> first_procedure;
        protected static bool first_procedure_can_perform = true;
        protected static Mock<ICanPerformBootProcedure> second_procedure;
        protected static bool second_procedure_can_perform = true;

        protected static BootProcedures boot_procedures;

        Establish context = () =>
        {
            first_procedure = new Mock<ICanPerformBootProcedure>();
            first_procedure.Setup(_ => _.CanPerform()).Returns(() => first_procedure_can_perform);
            second_procedure = new Mock<ICanPerformBootProcedure>();
            second_procedure.Setup(_ => _.CanPerform()).Returns(() => second_procedure_can_perform);

            var instances = new Mock<IInstancesOf<ICanPerformBootProcedure>>();
            var listOfInstances = new List<ICanPerformBootProcedure>();
            listOfInstances.AddRange(new[]
            {
                first_procedure.Object,
                second_procedure.Object
            });
            instances.Setup(_ => _.GetEnumerator()).Returns(listOfInstances.GetEnumerator());

            container.Setup(_ => _.Get<IInstancesOf<ICanPerformBootProcedure>>()).Returns(instances.Object);

            boot_procedures = new BootProcedures(instances.Object, Mock.Of<ILogger>(), execution_context_manager.Object);
        };
    }
}