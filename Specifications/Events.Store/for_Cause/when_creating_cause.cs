// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_Cause
{
    public class when_creating_cause
    {
        static CauseType type;
        static CauseLogPosition position;
        static Cause cause;

        Establish context = () =>
        {
            type = CauseType.Command;
            position = 0;
        };

        Because of = () => cause = new Cause(type, position);

        It should_have_the_correct_cause_type = () => cause.Type.ShouldEqual(type);
        It should_have_the_correct_cause_log_position = () => cause.Position.ShouldEqual(position);
    }
}