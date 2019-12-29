// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events;
using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource
{
    [Subject(typeof(EventSource))]
    public class when_fast_fowarding_a_stateful_aggregate_root : given.a_stateful_event_source
    {
        static Exception exception;
        static EventSourceVersion last_commit;

        Establish context = () => last_commit = new EventSourceVersion(1, 1);

        Because of = () => exception = Catch.Exception(() => event_source.FastForward(last_commit));

        It should_throw_an_fast_forward_not_allowed_for_stateful_event_source_exception = () => exception.ShouldBeOfExactType<FastForwardNoAllowedForStatefulEventSource>();
    }
}