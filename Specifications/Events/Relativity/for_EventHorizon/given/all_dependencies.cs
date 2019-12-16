// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon.given
{
    public class all_dependencies
    {
        protected static Mock<ILogger> logger;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IFetchUnprocessedCommits> unproccessed_commits_fetcher;

        Establish context = () =>
        {
            logger = new Mock<ILogger>();
            execution_context_manager = new Mock<IExecutionContextManager>();
            unproccessed_commits_fetcher = new Mock<IFetchUnprocessedCommits>();
       };
    }
}