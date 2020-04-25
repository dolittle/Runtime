// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoots.when_incrementing_version
{
    public class and_incrementing_once_with_initial_version_and_once_more_with_a_greater_version : given.all_dependencies
    {
        static AggregateRoots aggregate_roots;
        static AggregateRootVersion next_version;
        static EventSourceId event_source_id;
        static Artifacts.ArtifactId aggregate_root;
        static AggregateRoot result;
        static AggregateRoot persisted;

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
            result = aggregate_roots.IncrementVersionFor(session, event_source_id, aggregate_root, next_version, next_version + 3, CancellationToken.None).GetAwaiter().GetResult();
            persisted = an_event_store_connection.Aggregates.Find(
                filters.an_aggregate_filter.Eq(_ => _.EventSource, event_source_id.Value)
                & filters.an_aggregate_filter.Eq(_ => _.AggregateType, aggregate_root.Value)).FirstOrDefault();
        };

        It should_return_an_aggregate_root = () => result.ShouldNotBeNull();
        It should_have_the_correct_aggregate_type = () => result.AggregateType.ShouldEqual(aggregate_root.Value);
        It should_have_the_correct_event_source = () => result.EventSource.ShouldEqual(event_source_id.Value);
        It should_point_to_next_version_plus_three = () => result.Version.ShouldEqual(next_version.Value + 3);
        It should_have_persisted_aggregate_root = () => persisted.ShouldNotBeNull();
        It should_have_persisted_the_correct_aggregate_root_version = () => persisted.Version.ShouldEqual(result.Version);
        It should_have_persisted_the_correct_aggregate_event_source = () => persisted.EventSource.ShouldEqual(result.EventSource);
        It should_have_persisted_the_correct_aggregate_type = () => persisted.AggregateType.ShouldEqual(result.AggregateType);
    }
}