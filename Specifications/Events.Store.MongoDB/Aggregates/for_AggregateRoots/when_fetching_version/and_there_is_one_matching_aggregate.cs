// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoots.when_fetching_version
{
    public class and_there_is_one_matching_aggregate : given.all_dependencies
    {
        static AggregateRoots aggregate_roots;
        static EventSourceId event_source_id;
        static Artifacts.ArtifactId aggregate_root;
        static AggregateRootVersion next_version;
        static AggregateRootVersion result;

        Establish context = () =>
        {
            aggregate_roots = new AggregateRoots(an_event_store_connection);
            event_source_id = Guid.NewGuid();
            aggregate_root = Guid.NewGuid();
            next_version = 3;
            using var session = an_event_store_connection.MongoClient.StartSession();
            aggregate_roots.IncrementVersionFor(session, event_source_id, aggregate_root, AggregateRootVersion.Initial, next_version, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () =>
        {
            using var session = an_event_store_connection.MongoClient.StartSession();
            result = aggregate_roots.FetchVersionFor(session, event_source_id, aggregate_root, CancellationToken.None).GetAwaiter().GetResult();
        };

        It should_return_the_next_aggregate_root_version = () => result.ShouldEqual(next_version);
    }
}