// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_getting_on_methods : given.two_aggregate_roots
    {
        static MethodInfo simple_event_method_on_statless;
        static MethodInfo another_event_method_on_statless;
        static MethodInfo simple_event_method_on_statfull;
        static MethodInfo another_event_method_on_statfull;

        Because of = () =>
        {
            simple_event_method_on_statless = stateless_aggregate_root.GetOnMethod(event_one);
            another_event_method_on_statless = stateless_aggregate_root.GetOnMethod(event_three);
            simple_event_method_on_statfull = statefull_aggregate_root.GetOnMethod(event_two);
            another_event_method_on_statfull = statefull_aggregate_root.GetOnMethod(event_three);
        };

        It should_be_null_for_simple_event_on_stateless = () => simple_event_method_on_statless.ShouldBeNull();
        It should_be_null_for_another_event_on_stateless = () => another_event_method_on_statless.ShouldBeNull();
        It should_get_the_correct_method_for_simple_event_on_statefull = () => simple_event_method_on_statfull.ShouldEqual(statefull_aggregate_root.OnSimpleEventMethod);
        It should_get_the_correct_method_for_another_event_on_statefull = () => another_event_method_on_statfull.ShouldEqual(statefull_aggregate_root.OnAnotherEventMethod);
    }
}
