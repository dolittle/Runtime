// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events;
using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource
{
    [Subject(typeof(EventSource))]
    public class when_fast_fowarding_a_stateless_aggregate_root_where_there_is_no_current_version : given.a_stateless_event_source
    {
        static EventSourceVersion expected_version;

        Establish context = () => expected_version = EventSourceVersion.Initial;
        Because of = () => event_source.FastForward(null);
        It should_fast_forward_the_version = () => event_source.Version.ShouldEqual(expected_version);
    }
}