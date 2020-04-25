// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoots.when_incrementing_version
{
    public class and_next_version_is_less_than_expected_version : given.all_dependencies
    {
        static AggregateRoots aggregate_roots;
        static Exception exception;
        Establish context = () => aggregate_roots = new AggregateRoots(an_event_store_connection);

        Because of = () =>
        {
            using var session = an_event_store_connection.MongoClient.StartSession();
            exception = Catch.Exception(() => aggregate_roots.IncrementVersionFor(session, Guid.NewGuid(), Guid.NewGuid(), 1, 0, CancellationToken.None).GetAwaiter().GetResult());
        };

        It should_throw_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_next_version_is_less_than_expected_version = () => exception.ShouldBeOfExactType<NextAggregateRootVersionMustBeGreaterThanCurrentVersion>();
    }
}