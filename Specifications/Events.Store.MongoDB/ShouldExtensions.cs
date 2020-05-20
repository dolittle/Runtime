// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
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

        public static void ShouldBeTheSameAs(this Version storedVersion, Versioning.Version version)
        {
            storedVersion.Major.ShouldEqual(version.Major);
            storedVersion.Minor.ShouldEqual(version.Minor);
            storedVersion.Patch.ShouldEqual(version.Patch);
            storedVersion.Build.ShouldEqual(version.Build);
            storedVersion.PreRelease.ShouldEqual(version.PreReleaseString);
        }

        public static void ShouldBeTheSameAs(this Claim storedClaim, Security.Claim claim)
        {
            storedClaim.Name.ShouldEqual(claim.Name);
            storedClaim.Value.ShouldEqual(claim.Value);
            storedClaim.ValueType.ShouldEqual(claim.ValueType);
        }

        public static void ShouldBeTheSameAs(this IEnumerable<Claim> storedClaims, Security.Claims claims)
        {
            storedClaims.ShouldContainOnly(claims.Select(_ => _.ToStoreRepresentation()));
        }

        public static void ShouldBeTheSameAs(this Execution.ExecutionContext executionContext, ExecutionContext storedExecutionContext)
        {
            executionContext.CorrelationId.Value.ShouldEqual(storedExecutionContext.Correlation);
            executionContext.Environment.Value.ShouldEqual(storedExecutionContext.Environment);
            executionContext.Microservice.Value.ShouldEqual(storedExecutionContext.Microservice);
            executionContext.Tenant.Value.ShouldEqual(storedExecutionContext.Tenant);
            executionContext.Version.ShouldBeTheSameAs(storedExecutionContext.Version);
            executionContext.Claims.ShouldBeTheSameAs(storedExecutionContext.Claims);
        }

        public static void ShouldBeTheSameAs(this Versioning.Version version, Version storedVersion)
        {
            version.Major.ShouldEqual(storedVersion.Major);
            version.Minor.ShouldEqual(storedVersion.Minor);
            version.Patch.ShouldEqual(storedVersion.Patch);
            version.Build.ShouldEqual(storedVersion.Build);
            version.PreReleaseString.ShouldEqual(storedVersion.PreRelease);
        }

        public static void ShouldBeTheSameAs(this Security.Claim claim, Claim storedClaim)
        {
            claim.Name.ShouldEqual(storedClaim.Name);
            claim.Value.ShouldEqual(storedClaim.Value);
            claim.ValueType.ShouldEqual(storedClaim.ValueType);
        }

        public static void ShouldBeTheSameAs(this Security.Claims claims, IEnumerable<Claim> storedClaims)
        {
            claims.ShouldContainOnly(storedClaims.Select(_ => _.ToClaim()));
        }

        public static void ShouldBeTheSameAs(this CommittedEvent committedEvent, Events.Event storedEvent)
        {
            committedEvent.Content.ShouldEqual(storedEvent.Content.ToString());
            committedEvent.EventLogSequenceNumber.Value.ShouldEqual(storedEvent.EventLogSequenceNumber);
            committedEvent.EventSource.Value.ShouldEqual(storedEvent.Metadata.EventSource);
            committedEvent.ExecutionContext.ShouldBeTheSameAs(storedEvent.ExecutionContext);
            committedEvent.Occurred.UtcDateTime.ShouldEqual(storedEvent.Metadata.Occurred);
            committedEvent.Public.ShouldEqual(storedEvent.Metadata.Public);
            committedEvent.Type.Id.Value.ShouldEqual(storedEvent.Metadata.TypeId);
            committedEvent.Type.Generation.Value.ShouldEqual(storedEvent.Metadata.TypeGeneration);
        }

        public static void ShouldBeTheSameAs(this CommittedAggregateEvent committedEvent, Events.Event storedEvent)
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
            committedEvent.Received.UtcDateTime.ShouldEqual(storedEvent.EventHorizonMetadata.Received);
            committedEvent.Consent.Value.ShouldEqual(storedEvent.EventHorizonMetadata.Consent);
        }

        public static void ShouldBeTheSameAs(this CommittedEvent committedEvent, StreamEvent storedEvent)
        {
            committedEvent.Content.ShouldEqual(storedEvent.Content.ToString());
            committedEvent.EventLogSequenceNumber.Value.ShouldEqual(storedEvent.Metadata.EventLogSequenceNumber);
            committedEvent.EventSource.Value.ShouldEqual(storedEvent.Metadata.EventSource);
            committedEvent.ExecutionContext.ShouldBeTheSameAs(storedEvent.ExecutionContext);
            committedEvent.Occurred.UtcDateTime.ShouldEqual(storedEvent.Metadata.Occurred);
            committedEvent.Public.ShouldEqual(storedEvent.Metadata.Public);
            committedEvent.Type.Id.Value.ShouldEqual(storedEvent.Metadata.TypeId);
            committedEvent.Type.Generation.Value.ShouldEqual(storedEvent.Metadata.TypeGeneration);
        }

        public static void ShouldBeTheSameAs(this CommittedAggregateEvent committedEvent, StreamEvent storedEvent)
        {
            (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
            committedEvent.AggregateRoot.Id.Value.ShouldEqual(storedEvent.Aggregate.TypeId);
            committedEvent.AggregateRoot.Generation.Value.ShouldEqual(storedEvent.Aggregate.TypeGeneration);
            committedEvent.AggregateRootVersion.Value.ShouldEqual(storedEvent.Aggregate.Version);
        }

        public static void ShouldBeTheSameAs(this CommittedExternalEvent committedEvent, StreamEvent storedEvent)
        {
            (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
            committedEvent.ExternalEventLogSequenceNumber.Value.ShouldEqual(storedEvent.EventHorizonMetadata.ExternalEventLogSequenceNumber);
            committedEvent.Received.ShouldEqual(storedEvent.EventHorizonMetadata.Received);
            committedEvent.Consent.Value.ShouldEqual(storedEvent.EventHorizonMetadata.Consent);
        }

        public static void ShouldBeTheSameAs(this Events.Event storedEvent, CommittedEvent committedEvent)
        {
            storedEvent.Content.ToString().ShouldEqual(committedEvent.Content);
            storedEvent.EventLogSequenceNumber.ShouldEqual(committedEvent.EventLogSequenceNumber.Value);
            storedEvent.ExecutionContext.ShouldBeTheSameAs(committedEvent.ExecutionContext);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEvent.EventSource.Value);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEvent.Occurred.UtcDateTime);
            storedEvent.Metadata.Public.ShouldEqual(committedEvent.Public);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEvent.Type.Id.Value);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEvent.Type.Generation.Value);
        }

        public static void ShouldBeTheSameAs(this StreamEvent storedEvent, CommittedEvent committedEvent)
        {
            storedEvent.Content.ToString().ShouldEqual(committedEvent.Content);
            storedEvent.Metadata.EventLogSequenceNumber.ShouldEqual(committedEvent.EventLogSequenceNumber.Value);
            storedEvent.ExecutionContext.ShouldBeTheSameAs(committedEvent.ExecutionContext);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEvent.EventSource.Value);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEvent.Occurred.UtcDateTime);
            storedEvent.Metadata.Public.ShouldEqual(committedEvent.Public);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEvent.Type.Id.Value);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEvent.Type.Generation.Value);
        }
    }
}
