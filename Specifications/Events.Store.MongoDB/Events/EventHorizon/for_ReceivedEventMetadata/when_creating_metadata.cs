// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.ApplicationModel;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.EventHorizon.for_ReceivedEventMetadata
{
    public class when_creating_metadata
    {
        static DateTimeOffset occurred;
        static EventSourceId event_source;
        static CorrelationId correlation;
        static Microservice microservice;
        static TenantId consumer_tenant;
        static TenantId producer_tenant;
        static EventLogSequenceNumber origin_event_log_sequence_number;
        static ArtifactId type_id;
        static ArtifactGeneration type_generation;
        static EventHorizonEventMetadata metadata;

        Establish context = () =>
        {
            occurred = DateTimeOffset.Now;
            event_source = Guid.NewGuid();
            correlation = Guid.NewGuid();
            microservice = Guid.NewGuid();
            consumer_tenant = Guid.NewGuid();
            producer_tenant = Guid.NewGuid();
            origin_event_log_sequence_number = 0;
            type_id = Guid.NewGuid();
            type_generation = 1;
        };

        Because of = () =>
            metadata = new EventHorizonEventMetadata(
                occurred,
                event_source,
                correlation,
                microservice,
                consumer_tenant,
                producer_tenant,
                origin_event_log_sequence_number,
                type_id,
                type_generation);

        It should_have_the_correct_occurred_value = () => metadata.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_event_source = () => metadata.EventSource.ShouldEqual(event_source.Value);
        It should_have_the_correct_correlation = () => metadata.Correlation.ShouldEqual(correlation.Value);
        It should_have_the_correct_microservice = () => metadata.Microservice.ShouldEqual(microservice.Value);
        It should_have_the_correct_consumer_tenant = () => metadata.ConsumerTenant.ShouldEqual(consumer_tenant.Value);
        It should_have_the_correct_producer_tenant = () => metadata.ProducerTenant.ShouldEqual(producer_tenant.Value);
        It should_have_the_correct_origin_event_log_sequence_number = () => metadata.OriginEventLogSequenceNumber.ShouldEqual(origin_event_log_sequence_number.Value);
        It should_have_the_correct_type_id = () => metadata.TypeId.ShouldEqual(type_id.Value);
        It should_have_the_correct_type_generation = () => metadata.TypeGeneration.ShouldEqual(type_generation.Value);
    }
}