// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContext
{
    public class when_getting_objects_being_tracked : given.a_command_context_for_a_simple_command_with_one_tracked_object
    {
        static IEnumerable<IEventSource> result;

        Because of = () => result = command_context.GetObjectsBeingTracked();

        It should_return_one_aggregated_root = () => result.Count().ShouldEqual(1);
        It should_have_the_expected_aggreated_root = () => result.First().ShouldEqual(aggregated_root);
    }
}
