// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventCommitter
{
    public class when_committing_event : given.all_dependencies
    {
        static EventCommitter event_committer;
        static EventLogSequenceNumber event_log_sequence_number;
        static DateTimeOffset occurred;
        static Cause cause;
        static CommittedEvent result;
        static Event persisted_event;

        Establish context = () =>
        {
            event_committer = new EventCommitter(an_event_store_connection);
            event_log_sequence_number = 0;
            occurred = DateTimeOffset.UtcNow;
            cause = new Cause(CauseType.Command, 0U);
        };

        Because of = () =>
        {
            using var session = an_event_store_connection.MongoClient.StartSession();
            result = event_committer.CommitEvent(session, event_log_sequence_number, occurred, execution_context, cause, uncommitted_event).GetAwaiter().GetResult();
            persisted_event = an_event_store_connection.EventLog.Find(filters.event_filter.Empty).FirstOrDefault();
        };

        It should_return_committed_event = () => result.ShouldNotBeNull();
        It should_have_the_same_event_content = () => result.Content.ShouldEqual(uncommitted_event.Content);
        It should_be_the_same_artifact = () => result.Type.ShouldEqual(uncommitted_event.Type);
        It should_have_the_correct_cause = () => result.Cause.ShouldEqual(cause);
        It should_have_the_correct_correlation_id = () => result.CorrelationId.ShouldEqual(execution_context.CorrelationId);
        It should_have_event_log_sequence_number_zero = () => result.EventLogSequenceNumber.Value.ShouldEqual(0U);
        It should_have_a_not_set_event_source = () => result.EventSource.ShouldEqual(EventSourceId.NotSet);
        It should_have_the_correct_microservice = () => result.Microservice.Value.ShouldEqual(execution_context.BoundedContext);
        It should_have_the_correct_occurred_time = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_tenant = () => result.Tenant.ShouldEqual(execution_context.Tenant);
        It should_be_persisted_in_all_stream_position_one = () => persisted_event.ShouldNotBeNull();
        It should_have_persisted_the_correct_event = () => result.ShouldBeStoredWithCorrectStoreRepresentation(persisted_event, 0, PartitionId.NotSet);
    }
}