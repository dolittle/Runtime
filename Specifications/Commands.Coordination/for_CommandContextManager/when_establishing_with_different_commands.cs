// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContextManager
{
    [Subject(Subjects.establishing_context)]
    public class when_establishing_with_different_commands : given.a_command_context_manager
    {
        static ICommandContext firstCommandContext;
        static ICommandContext secondCommandContext;

        Because of = () =>
                         {
                             var first_artifact = Artifact.New();
                             var second_artifact = Artifact.New();
                             var firstCommand = new CommandRequest(CorrelationId.Empty, first_artifact.Id, first_artifact.Generation, new ExpandoObject());
                             var secondCommand = new CommandRequest(CorrelationId.Empty, second_artifact.Id, second_artifact.Generation, new ExpandoObject());
                             firstCommandContext = Manager.EstablishForCommand(firstCommand);
                             secondCommandContext = Manager.EstablishForCommand(secondCommand);
                         };

        It should_return_different_contexts = () => firstCommandContext.ShouldNotEqual(secondCommandContext);
    }
}