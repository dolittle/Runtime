// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventCommitter
{
    public class when_committing_aggregate_event : given.all_dependencies
    {
        static EventCommitter event_committer;
        static EventLogSequenceNumber event_log_sequence_number;
        static DateTimeOffset occurred;
        static AggregateRootVersion aggregate_root_version;
        static Artifacts.Artifact aggregate_root;
        static CommittedAggregateEvent result;
        static Event persisted_event;

        Establish context = () =>
        {
            event_committer = new EventCommitter(an_event_store_connection);
            event_log_sequence_number = 0;
            occurred = DateTimeOffset.UtcNow;
            aggregate_root_version = 0U;
            aggregate_root = new Artifacts.Artifact(Guid.NewGuid(), 0);
        };

        Because of = () =>
        {
            using var session = an_event_store_connection.MongoClient.StartSession();
            result = event_committer.CommitAggregateEvent(session, aggregate_root, aggregate_root_version, event_log_sequence_number, occurred, event_source, execution_context, uncommitted_event, CancellationToken.None).GetAwaiter().GetResult();
            persisted_event = an_event_store_connection.EventLog.Find(filters.event_filter.Empty).FirstOrDefault();
        };

        It should_return_committed_event = () => result.ShouldNotBeNull();
        It should_have_the_same_event_content = () => result.Content.ShouldEqual(uncommitted_event.Content);
        It should_be_the_same_artifact = () => result.Type.ShouldEqual(uncommitted_event.Type);
        It should_have_the_correct_correlation_id = () => result.ExecutionContext.CorrelationId.ShouldEqual(execution_context.CorrelationId);
        It should_have_event_log_sequence_number_zero = () => result.EventLogSequenceNumber.ShouldEqual(0U);
        It should_have_a_not_set_event_source = () => result.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_microservice = () => result.ExecutionContext.Microservice.ShouldEqual(execution_context.Microservice);
        It should_have_the_correct_occurred_time = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_tenant = () => result.ExecutionContext.Tenant.ShouldEqual(execution_context.Tenant);
        It should_have_the_correct_aggregate_root = () => result.AggregateRoot.ShouldEqual(aggregate_root);
        It should_have_the_correct_aggregate_root_version = () => result.AggregateRootVersion.ShouldEqual(aggregate_root_version);
        It should_be_persisted_in_all_stream_position_one = () => persisted_event.ShouldNotBeNull();
        It should_have_persisted_the_correct_event = () => result.ShouldBeStoredWithCorrectStoreRepresentation(persisted_event, 0, PartitionId.NotSet);
    }
}