// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Logging;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Specs
{
    public static class mocks
    {
        public static Mock<ILogger> a_logger()
        {
            var logger = new Mock<ILogger>();
            return logger;
        }

        public static Mock<IExecutionContextManager> an_execution_context_manager()
        {
            var mgr = new Mock<IExecutionContextManager>();
            return mgr;
        }
    }
}