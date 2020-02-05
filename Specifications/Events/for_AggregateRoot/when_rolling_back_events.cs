// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_rolling_back_events : given.two_aggregate_roots
    {
        Establish context = () =>
        {
            stateless_aggregate_root.Apply(event_one);
            stateless_aggregate_root.Apply(event_two);
            stateless_aggregate_root.Apply(event_three);
        };

        Because of = () =>
        {
            stateless_aggregate_root.Rollback();
        };

        It should_have_no_uncommitted_events = () => stateless_aggregate_root.UncommittedEvents.ShouldBeEmpty();
        It should_be_at_the_initial_version = () => stateless_aggregate_root.Version.ShouldEqual(AggregateRootVersion.Initial);
    }
}
