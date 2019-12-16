// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events;
using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource
{
    [Subject(typeof(EventSource))]
    public class when_fast_fowarding_a_stateless_aggregate_root_that_is_not_the_initial_version : given.a_stateless_event_source
    {
        static InvalidFastForward exception;
        static EventSourceVersion last_commit;

        Establish context = () =>
                                {
                                    last_commit = new EventSourceVersion(1, 1);
                                    event_source.Commit();
                                };

        Because of = () => exception = Catch.Exception(() => event_source.FastForward(last_commit)) as InvalidFastForward;

        It should_throw_an_invalid_fast_forward_exception = () => exception.ShouldNotBeNull();
    }
}