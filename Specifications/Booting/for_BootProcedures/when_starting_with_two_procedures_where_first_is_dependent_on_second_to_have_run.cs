// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extension.Logging;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Booting.Specs.for_BootProcedures
{
    public class when_starting_with_two_procedures_where_first_is_dependent_on_second_to_have_run : given.two_procedures
    {
        static ICanPerformBootProcedure last_boot_procedure;

        Establish context = () =>
        {
            first_procedure.Setup(_ => _.Perform()).Callback(() => last_boot_procedure = first_procedure.Object);
            second_procedure.Setup(_ => _.Perform()).Callback(() =>
            {
                last_boot_procedure = second_procedure.Object;
                first_procedure_can_perform = true;
            });
            first_procedure_can_perform = false;
        };

        Because of = () => boot_procedures.Perform();

        It should_perform_second_then_first = () => last_boot_procedure.ShouldEqual(first_procedure.Object);
    }
}