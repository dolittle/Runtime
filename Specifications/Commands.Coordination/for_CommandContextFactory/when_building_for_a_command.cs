// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContextFactory
{
    [Subject(typeof(CommandContextFactory))]
    public class when_building_for_a_command : given.a_command_context_factory
    {
        static CommandRequest command;
        static ICommandContext command_context;

        Establish context = () =>
            {
                var artifact = Artifact.New();
                command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
            };

        Because of = () => command_context = factory.Build(command);

        It should_build_a_commmand_context = () => command_context.ShouldBeOfExactType<CommandContext>();
        It should_contain_the_commmand = () => command_context.Command.ShouldEqual(command);
    }
}