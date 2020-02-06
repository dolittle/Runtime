// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_fast_forwarding_a_statefull_aggregate_root : given.two_aggregate_roots
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
        {
            statefull_aggregate_root.FastForward(1);
        });

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<FastForwardNotAllowedForStatefulAggregateRoot>();
    }
}
