// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions.when_converting_to_committed_event
{
    public class from_stream_event_from_event_horizon
    {
        static StreamEvent @event;
        static CommittedEvent result;

        Establish context = () =>
        {
            @event = events.new_stream_event_not_from_aggregate(0, Guid.NewGuid()).from_event_horizon().build();
        };

        Because of = () => result = @event.ToCommittedEvent();

        It should_return_a_committed_event = () => result.ShouldBeOfExactType<CommittedExternalEvent>();
        It should_have_the_correct_content = () => result.Content.ShouldEqual(@event.Content.ToString());
        It should_have_the_correct_event_log_sequence_number = () => result.EventLogSequenceNumber.Value.ShouldEqual(@event.Metadata.EventLogSequenceNumber);
        It should_have_the_correct_event_source = () => result.EventSource.Value.ShouldEqual(@event.Metadata.EventSource);
        It should_have_the_correct_correlation = () => result.ExecutionContext.CorrelationId.Value.ShouldEqual(@event.ExecutionContext.Correlation);
        It should_have_the_correct_environment = () => result.ExecutionContext.Environment.Value.ShouldEqual(@event.ExecutionContext.Environment);
        It should_have_the_correct_microservice = () => result.ExecutionContext.Microservice.Value.ShouldEqual(@event.ExecutionContext.Microservice);
        It should_have_the_correct_tenant = () => result.ExecutionContext.Tenant.Value.ShouldEqual(@event.ExecutionContext.Tenant);

        It should_have_the_correct_version = () =>
            {
                result.ExecutionContext.Version.Major.ShouldEqual(@event.ExecutionContext.Version.Major);
                result.ExecutionContext.Version.Minor.ShouldEqual(@event.ExecutionContext.Version.Minor);
                result.ExecutionContext.Version.Patch.ShouldEqual(@event.ExecutionContext.Version.Patch);
                result.ExecutionContext.Version.Build.ShouldEqual(@event.ExecutionContext.Version.Build);
                result.ExecutionContext.Version.PreReleaseString.ShouldEqual(@event.ExecutionContext.Version.PreRelease);
            };

        It should_have_the_correct_occurred_value = () => result.Occurred.ShouldEqual(@event.Metadata.Occurred);
        It should_have_the_correct_public_value = () => result.Public.ShouldEqual(@event.Metadata.Public);

        It should_have_the_correct_type = () =>
            {
                result.Type.Id.Value.ShouldEqual(@event.Metadata.TypeId);
                result.Type.Generation.Value.ShouldEqual(@event.Metadata.TypeGeneration);
            };

        It should_have_the_correct_consent = () => (result as CommittedExternalEvent).Consent.Value.ShouldEqual(@event.EventHorizonMetadata.Consent);
        It should_have_the_correct_external_event_log_sequence_number = () => (result as CommittedExternalEvent).ExternalEventLogSequenceNumber.Value.ShouldEqual(@event.EventHorizonMetadata.ExternalEventLogSequenceNumber);
        It should_have_the_correct_received_value = () => (result as CommittedExternalEvent).Received.ShouldEqual(@event.EventHorizonMetadata.Received);
    }
}