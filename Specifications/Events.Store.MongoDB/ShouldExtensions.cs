// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class ShouldExtensions
    {
        public static void ShouldBeTheSameAs(this ExecutionContext storedExecutionContext, Execution.ExecutionContext executionContext)
        {
            storedExecutionContext.Correlation.ShouldEqual(executionContext.CorrelationId.Value);
            storedExecutionContext.Environment.ShouldEqual(executionContext.Environment.Value);
            storedExecutionContext.Microservice.ShouldEqual(executionContext.Microservice.Value);
            storedExecutionContext.Tenant.ShouldEqual(executionContext.Tenant.Value);
            storedExecutionContext.Version.ShouldBeTheSameAs(executionContext.Version);
        }

        public static void ShouldBeTheSameAs(this MongoDB.Events.Version storedVersion, Dolittle.Versioning.Version version)
        {
            storedVersion.Major.ShouldEqual(version.Major);
            storedVersion.Minor.ShouldEqual(version.Minor);
            storedVersion.Patch.ShouldEqual(version.Patch);
            storedVersion.Build.ShouldEqual(version.Build);
            storedVersion.PreRelease.ShouldEqual(version.PreReleaseString);
        }

        public static void ShouldBeTheSameAs(this Execution.ExecutionContext executionContext, ExecutionContext storedExecutionContext)
        {
            executionContext.CorrelationId.Value.ShouldEqual(storedExecutionContext.Correlation);
            executionContext.Environment.Value.ShouldEqual(storedExecutionContext.Environment);
            executionContext.Microservice.Value.ShouldEqual(storedExecutionContext.Microservice);
            executionContext.Tenant.Value.ShouldEqual(storedExecutionContext.Tenant);
            executionContext.Version.ShouldBeTheSameAs(storedExecutionContext.Version);
        }

        public static void ShouldBeTheSameAs(this Dolittle.Versioning.Version version, MongoDB.Events.Version storedVersion)
        {
            version.Major.ShouldEqual(storedVersion.Major);
            version.Minor.ShouldEqual(storedVersion.Minor);
            version.Patch.ShouldEqual(storedVersion.Patch);
            version.Build.ShouldEqual(storedVersion.Build);
            version.PreReleaseString.ShouldEqual(storedVersion.PreRelease);
        }

        public static void ShouldBeTheSameAs(this CommittedEvent committedEvent, MongoDB.Events.Event storedEvent)
        {
            committedEvent.Content.ShouldEqual(storedEvent.Content.ToString());
            committedEvent.EventLogSequenceNumber.Value.ShouldEqual(storedEvent.EventLogSequenceNumber);
            committedEvent.EventSource.Value.ShouldEqual(storedEvent.Metadata.EventSource);
            committedEvent.ExecutionContext.ShouldBeTheSameAs(storedEvent.ExecutionContext);
            committedEvent.Occurred.ShouldEqual(storedEvent.Metadata.Occurred);
            committedEvent.Public.ShouldEqual(storedEvent.Metadata.Public);
            committedEvent.Type.Id.Value.ShouldEqual(storedEvent.Metadata.TypeId);
            committedEvent.Type.Generation.Value.ShouldEqual(storedEvent.Metadata.TypeGeneration);
        }

        public static void ShouldBeTheSameAs(this CommittedAggregateEvent committedEvent, MongoDB.Events.Event storedEvent)
        {
            (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
            committedEvent.AggregateRoot.Id.Value.ShouldEqual(storedEvent.Aggregate.TypeId);
            committedEvent.AggregateRoot.Generation.Value.ShouldEqual(storedEvent.Aggregate.TypeGeneration);
            committedEvent.AggregateRootVersion.Value.ShouldEqual(storedEvent.Aggregate.Version);
        }

        public static void ShouldBeTheSameAs(this CommittedExternalEvent committedEvent, MongoDB.Events.Event storedEvent)
        {
            (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
            committedEvent.ExternalEventLogSequenceNumber.Value.ShouldEqual(storedEvent.EventHorizonMetadata.ExternalEventLogSequenceNumber);
            committedEvent.Received.ShouldEqual(storedEvent.EventHorizonMetadata.Received);
            committedEvent.Consent.Value.ShouldEqual(storedEvent.EventHorizonMetadata.Consent);
        }

        public static void ShouldBeTheSameAs(this CommittedEvent committedEvent, MongoDB.Events.StreamEvent storedEvent)
        {
            committedEvent.Content.ShouldEqual(storedEvent.Content.ToString());
            committedEvent.EventLogSequenceNumber.Value.ShouldEqual(storedEvent.Metadata.EventLogSequenceNumber);
            committedEvent.EventSource.Value.ShouldEqual(storedEvent.Metadata.EventSource);
            committedEvent.ExecutionContext.ShouldBeTheSameAs(storedEvent.ExecutionContext);
            committedEvent.Occurred.ShouldEqual(storedEvent.Metadata.Occurred);
            committedEvent.Public.ShouldEqual(storedEvent.Metadata.Public);
            committedEvent.Type.Id.Value.ShouldEqual(storedEvent.Metadata.TypeId);
            committedEvent.Type.Generation.Value.ShouldEqual(storedEvent.Metadata.TypeGeneration);
        }

        public static void ShouldBeTheSameAs(this CommittedAggregateEvent committedEvent, MongoDB.Events.StreamEvent storedEvent)
        {
            (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
            committedEvent.AggregateRoot.Id.Value.ShouldEqual(storedEvent.Aggregate.TypeId);
            committedEvent.AggregateRoot.Generation.Value.ShouldEqual(storedEvent.Aggregate.TypeGeneration);
            committedEvent.AggregateRootVersion.Value.ShouldEqual(storedEvent.Aggregate.Version);
        }

        public static void ShouldBeTheSameAs(this CommittedExternalEvent committedEvent, MongoDB.Events.StreamEvent storedEvent)
        {
            (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
            committedEvent.ExternalEventLogSequenceNumber.Value.ShouldEqual(storedEvent.EventHorizonMetadata.ExternalEventLogSequenceNumber);
            committedEvent.Received.ShouldEqual(storedEvent.EventHorizonMetadata.Received);
            committedEvent.Consent.Value.ShouldEqual(storedEvent.EventHorizonMetadata.Consent);
        }

        public static void ShouldBeTheSameAs(this MongoDB.Events.Event storedEvent, CommittedEvent committedEvent)
        {
            storedEvent.Content.ToString().ShouldEqual(committedEvent.Content);
            storedEvent.EventLogSequenceNumber.ShouldEqual(committedEvent.EventLogSequenceNumber.Value);
            storedEvent.ExecutionContext.ShouldBeTheSameAs(committedEvent.ExecutionContext);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEvent.EventSource.Value);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEvent.Occurred);
            storedEvent.Metadata.Public.ShouldEqual(committedEvent.Public);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEvent.Type.Id.Value);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEvent.Type.Generation.Value);
        }

        public static void ShouldBeTheSameAs(
            this MongoDB.Events.StreamEvent storedEvent,
            CommittedEvent committedEvent)
        {
            storedEvent.Content.ToString().ShouldEqual(committedEvent.Content);
            storedEvent.Metadata.EventLogSequenceNumber.ShouldEqual(committedEvent.EventLogSequenceNumber.Value);
            storedEvent.ExecutionContext.ShouldBeTheSameAs(committedEvent.ExecutionContext);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEvent.EventSource.Value);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEvent.Occurred);
            storedEvent.Metadata.Public.ShouldEqual(committedEvent.Public);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEvent.Type.Id.Value);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEvent.Type.Generation.Value);
        }
    }
}
