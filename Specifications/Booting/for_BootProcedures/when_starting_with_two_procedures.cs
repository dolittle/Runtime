// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extension.Logging;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Booting.Specs.for_BootProcedures
{
    public class when_starting_with_two_procedures : given.two_procedures
    {
        Because of = () => boot_procedures.Perform();

        It should_perform_first_procedure = () => first_procedure.Verify(_ => _.Perform(), Moq.Times.Once());
        It should_perform_second_procedure = () => second_procedure.Verify(_ => _.Perform(), Moq.Times.Once());
    }
}