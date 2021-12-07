// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Booting.Specs.for_BootProcedures.given;

public class all_dependencies
{
    protected static Mock<IContainer> container;
    protected static Mock<ILogger> logger;

    protected static Mock<IExecutionContextManager> execution_context_manager;

    Establish context = () =>
    {
        logger = new Mock<ILogger>();
        container = new Mock<IContainer>();
        container.Setup(_ => _.Get<ILogger>()).Returns(logger.Object);
        execution_context_manager = new Mock<IExecutionContextManager>();
        container.Setup(_ => _.Get<IExecutionContextManager>()).Returns(execution_context_manager.Object);
    };
}