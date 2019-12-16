// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource
{
    [Subject(typeof(EventSourceExtensions))]
    public class when_checking_if_stateless_on_a_stateful_event_source : given.a_stateful_event_source
    {
        static bool is_stateless;

        Because of = () => is_stateless = event_source.IsStateless();

        It should_not_be_stateless = () => is_stateless.ShouldBeFalse();
    }
}