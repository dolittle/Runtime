// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_checking_for_statelessness : given.two_aggregate_roots
    {
        static bool stateless_is_stateless;
        static bool statefull_is_stateless;

        Because of = () =>
        {
            stateless_is_stateless = stateless_aggregate_root.IsStateless();
            statefull_is_stateless = statefull_aggregate_root.IsStateless();
        };

        It should_be_true_for_stateless_aggregate_root = () => stateless_is_stateless.ShouldBeTrue();
        It should_be_false_for_statefull_aggrgate_root = () => statefull_is_stateless.ShouldBeFalse();
    }
}
