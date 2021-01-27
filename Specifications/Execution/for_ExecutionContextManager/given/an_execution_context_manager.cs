// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extension.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Execution.for_ExecutionContextManager.given
{
    public class an_execution_context_manager
    {
        protected static ExecutionContextManager execution_context_manager;
        protected static ILogger logger;

        Establish context = () =>
        {
            logger = Mock.Of<ILogger>();
            execution_context_manager = new ExecutionContextManager(logger);
        };
    }
}