// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoots.when_fetching_version
{
    public class and_there_are_no_matching_aggregate : given.all_dependencies
    {
        static AggregateRoots aggregate_roots;
        static EventSourceId event_source_id;
        static Artifacts.ArtifactId aggregate_root;
        static AggregateRootVersion result;

        Establish context = () =>
        {
            aggregate_roots = new AggregateRoots(an_event_store_connection);
            event_source_id = Guid.NewGuid();
            aggregate_root = Guid.NewGuid();
        };

        Because of = () =>
        {
            using var session = an_event_store_connection.MongoClient.StartSession();
            result = aggregate_roots.FetchVersionFor(session, event_source_id, aggregate_root, CancellationToken.None).GetAwaiter().GetResult();
        };

        It should_return_the_initial_aggregate_root_version = () => result.ShouldEqual(AggregateRootVersion.Initial);
    }
}