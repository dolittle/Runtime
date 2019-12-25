// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Coordination;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContext.given
{
    public class a_command_context_for_a_simple_command
    {
        protected static CommandRequest command;
        protected static CommandContext command_context;
        protected static Mock<IUncommittedEventStreamCoordinator> uncommitted_event_stream_coordinator;
        protected static Mock<ILogger> logger;

        Establish context = () =>
        {
            var artifact = Artifact.New();
            command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
            uncommitted_event_stream_coordinator = new Mock<IUncommittedEventStreamCoordinator>();
            logger = new Mock<ILogger>();
            command_context = new CommandContext(command, null, uncommitted_event_stream_coordinator.Object, logger.Object);
        };
    }
}
