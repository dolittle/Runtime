// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContext.given
{
    public class a_command_context_for_a_simple_command_with_one_tracked_object_with_one_uncommitted_event : a_command_context_for_a_simple_command_with_one_tracked_object
    {
        protected static SimpleEvent uncommitted_event;

        Establish context = () =>
        {
            uncommitted_event = new SimpleEvent();
            aggregated_root.Apply(uncommitted_event);
        };
    }
}
