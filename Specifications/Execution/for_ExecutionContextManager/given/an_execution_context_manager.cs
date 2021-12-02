// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Execution.for_ExecutionContextManager.given;

public class an_execution_context_manager
{
    protected static ExecutionContextManager execution_context_manager;
    protected static ILogger logger;

    Establish context = () =>
    {
        logger = Mock.Of<ILogger>();
        execution_context_manager = new ExecutionContextManager(logger);
        execution_context_manager.CurrentFor(Microservice.NotSet, TenantId.Unknown, CorrelationId.System, "", 0, "");
    };
}