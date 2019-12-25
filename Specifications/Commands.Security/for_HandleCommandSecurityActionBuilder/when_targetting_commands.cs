// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Commands.Security.Specs.for_HandleCommandSecurityActionBuilder
{
    public class when_targetting_commands
    {
        static HandleCommand action;
        static CommandSecurityTarget target;

        Establish context = () => action = new HandleCommand();

        Because of = () => target = action.Commands();

        It should_return_a_command_security_target_builder = () => target.ShouldNotBeNull();
        It should_add_a_command_security_target = () => action.Targets.First().ShouldBeOfExactType<CommandSecurityTarget>();
    }
}
