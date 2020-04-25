// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoots.when_incrementing_version
{
    public class and_incrementing_twice_for_same_aggregate_with_initial_version : given.all_dependencies
    {
        static AggregateRoots aggregate_roots;
        static AggregateRootVersion next_version;
        static EventSourceId event_source_id;
        static Artifacts.ArtifactId aggregate_root;
        static Exception exception;

        Establish context = () =>
        {
            aggregate_roots = new AggregateRoots(an_event_store_connection);
            next_version = 3;
            event_source_id = Guid.NewGuid();
            aggregate_root = Guid.NewGuid();
            using var session = an_event_store_connection.MongoClient.StartSession();
            aggregate_roots.IncrementVersionFor(session, event_source_id, aggregate_root, AggregateRootVersion.Initial, next_version, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () =>
        {
            using var session = an_event_store_connection.MongoClient.StartSession();
            exception = Catch.Exception(() => aggregate_roots.IncrementVersionFor(session, event_source_id, aggregate_root, AggregateRootVersion.Initial, next_version, CancellationToken.None).GetAwaiter().GetResult());
        };

        It should_throw_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_due_to_a_concurrency_conflict = () => exception.ShouldBeOfExactType<AggregateRootConcurrencyConflict>();
    }
}