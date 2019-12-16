// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using System.Threading;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContextManager
{
    [Subject(Subjects.establishing_context)]
    public class when_establishing_on_different_threads_with_same_command : given.a_command_context_manager
    {
        static ICommandContext firstCommandContext;
        static ICommandContext secondCommandContext;

        Establish context = () =>
                                {
                                    var resetEvent = new ManualResetEvent(false);
                                    var artifact = Artifact.New();
                                    var command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
                                    firstCommandContext = Manager.EstablishForCommand(command);
                                    var thread = new Thread(
                                        () =>
                                            {
                                                secondCommandContext = Manager.EstablishForCommand(command);
                                                resetEvent.Reset();
                                            });
                                    thread.Start();
                                    resetEvent.WaitOne(1000);
                                };

        It should_return_different_contexts = () => firstCommandContext.ShouldNotEqual(secondCommandContext);
    }
}