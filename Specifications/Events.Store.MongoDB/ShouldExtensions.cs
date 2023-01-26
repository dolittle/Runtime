// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public static class ShouldExtensions
{
    public static void ShouldBeTheSameAs(this ExecutionContext storedExecutionContext, Execution.ExecutionContext executionContext)
    {
        storedExecutionContext.Correlation.Should().Be(executionContext.CorrelationId.Value);
        storedExecutionContext.Environment.Should().Be(executionContext.Environment.Value);
        storedExecutionContext.Microservice.Should().Be(executionContext.Microservice.Value);
        storedExecutionContext.Tenant.Should().Be(executionContext.Tenant.Value);
        storedExecutionContext.Version.ShouldBeTheSameAs(executionContext.Version);
    }

    public static void ShouldBeTheSameAs(this Version storedVersion, Domain.Platform.Version version)
    {
        storedVersion.Major.Should().Be(version.Major);
        storedVersion.Minor.Should().Be(version.Minor);
        storedVersion.Patch.Should().Be(version.Patch);
        storedVersion.Build.Should().Be(version.Build);
        storedVersion.PreRelease.Should().Be(version.PreReleaseString);
    }

    public static void ShouldBeTheSameAs(this Claim storedClaim, Execution.Claim claim)
    {
        storedClaim.Name.Should().Be(claim.Name);
        storedClaim.Value.Should().Be(claim.Value);
        storedClaim.ValueType.Should().Be(claim.ValueType);
    }

    public static void ShouldBeTheSameAs(this IEnumerable<Claim> storedClaims, Execution.Claims claims)
    {
        storedClaims.ShouldContainOnly(claims.Select(_ => _.ToStoreRepresentation()));
    }

    public static void ShouldBeTheSameAs(this Execution.ExecutionContext executionContext, ExecutionContext storedExecutionContext)
    {
        executionContext.CorrelationId.Value.Should().Be(storedExecutionContext.Correlation);
        executionContext.Environment.Value.Should().Be(storedExecutionContext.Environment);
        executionContext.Microservice.Value.Should().Be(storedExecutionContext.Microservice);
        executionContext.Tenant.Value.Should().Be(storedExecutionContext.Tenant);
        executionContext.Version.ShouldBeTheSameAs(storedExecutionContext.Version);
        executionContext.Claims.ShouldBeTheSameAs(storedExecutionContext.Claims);
    }

    public static void ShouldBeTheSameAs(this Domain.Platform.Version version, Version storedVersion)
    {
        version.Major.Should().Be(storedVersion.Major);
        version.Minor.Should().Be(storedVersion.Minor);
        version.Patch.Should().Be(storedVersion.Patch);
        version.Build.Should().Be(storedVersion.Build);
        version.PreReleaseString.Should().Be(storedVersion.PreRelease);
    }

    public static void ShouldBeTheSameAs(this Execution.Claim claim, Claim storedClaim)
    {
        claim.Name.Should().Be(storedClaim.Name);
        claim.Value.Should().Be(storedClaim.Value);
        claim.ValueType.Should().Be(storedClaim.ValueType);
    }

    public static void ShouldBeTheSameAs(this Execution.Claims claims, IEnumerable<Claim> storedClaims)
    {
        claims.ShouldContainOnly(storedClaims.Select(_ => _.ToClaim()));
    }

    public static void ShouldBeTheSameAs(this CommittedEvent committedEvent, Events.Event storedEvent)
    {
        committedEvent.EventLogSequenceNumber.Value.Should().Be(storedEvent.EventLogSequenceNumber);
        committedEvent.EventSource.Value.Should().Be(storedEvent.Metadata.EventSource);
        committedEvent.ExecutionContext.ShouldBeTheSameAs(storedEvent.ExecutionContext);
        committedEvent.Occurred.UtcDateTime.Should().Be(storedEvent.Metadata.Occurred);
        committedEvent.Public.Should().Be(storedEvent.Metadata.Public);
        committedEvent.Type.Id.Value.Should().Be(storedEvent.Metadata.TypeId);
        committedEvent.Type.Generation.Value.Should().Be(storedEvent.Metadata.TypeGeneration);
    }

    public static void ShouldBeTheSameAs(this CommittedAggregateEvent committedEvent, Events.Event storedEvent)
    {
        (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
        committedEvent.AggregateRoot.Id.Value.Should().Be(storedEvent.Aggregate.TypeId);
        committedEvent.AggregateRoot.Generation.Value.Should().Be(storedEvent.Aggregate.TypeGeneration);
        committedEvent.AggregateRootVersion.Value.Should().Be(storedEvent.Aggregate.Version);
    }

    public static void ShouldBeTheSameAs(this CommittedExternalEvent committedEvent, MongoDB.Events.Event storedEvent)
    {
        (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
        committedEvent.ExternalEventLogSequenceNumber.Value.Should().Be(storedEvent.EventHorizon.ExternalEventLogSequenceNumber);
        committedEvent.Received.UtcDateTime.Should().Be(storedEvent.EventHorizon.Received);
        committedEvent.Consent.Value.Should().Be(storedEvent.EventHorizon.Consent);
    }

    public static void ShouldBeTheSameAs(this CommittedEvent committedEvent, StreamEvent storedEvent)
    {
        committedEvent.EventLogSequenceNumber.Value.Should().Be(storedEvent.Metadata.EventLogSequenceNumber);
        committedEvent.EventSource.Value.Should().Be(storedEvent.Metadata.EventSource);
        committedEvent.ExecutionContext.ShouldBeTheSameAs(storedEvent.ExecutionContext);
        committedEvent.Occurred.UtcDateTime.Should().Be(storedEvent.Metadata.Occurred);
        committedEvent.Public.Should().Be(storedEvent.Metadata.Public);
        committedEvent.Type.Id.Value.Should().Be(storedEvent.Metadata.TypeId);
        committedEvent.Type.Generation.Value.Should().Be(storedEvent.Metadata.TypeGeneration);
    }

    public static void ShouldBeTheSameAs(this CommittedAggregateEvent committedEvent, StreamEvent storedEvent)
    {
        (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
        committedEvent.AggregateRoot.Id.Value.Should().Be(storedEvent.Aggregate.TypeId);
        committedEvent.AggregateRoot.Generation.Value.Should().Be(storedEvent.Aggregate.TypeGeneration);
        committedEvent.AggregateRootVersion.Value.Should().Be(storedEvent.Aggregate.Version);
    }

    public static void ShouldBeTheSameAs(this CommittedExternalEvent committedEvent, StreamEvent storedEvent)
    {
        (committedEvent as CommittedEvent).ShouldBeTheSameAs(storedEvent);
        committedEvent.ExternalEventLogSequenceNumber.Value.Should().Be(storedEvent.EventHorizon.ExternalEventLogSequenceNumber);
        committedEvent.Received.Should().Be(storedEvent.EventHorizon.Received);
        committedEvent.Consent.Value.Should().Be(storedEvent.EventHorizon.Consent);
    }

    public static void ShouldBeTheSameAs(this Events.Event storedEvent, CommittedEvent committedEvent)
    {
        storedEvent.EventLogSequenceNumber.Should().Be(committedEvent.EventLogSequenceNumber.Value);
        storedEvent.ExecutionContext.ShouldBeTheSameAs(committedEvent.ExecutionContext);
        storedEvent.Metadata.EventSource.Should().Be(committedEvent.EventSource.Value);
        storedEvent.Metadata.Occurred.Should().Be(committedEvent.Occurred.UtcDateTime);
        storedEvent.Metadata.Public.Should().Be(committedEvent.Public);
        storedEvent.Metadata.TypeId.Should().Be(committedEvent.Type.Id.Value);
        storedEvent.Metadata.TypeGeneration.Should().Be(committedEvent.Type.Generation.Value);
    }

    public static void ShouldBeTheSameAs(this StreamEvent storedEvent, CommittedEvent committedEvent)
    {
        storedEvent.Metadata.EventLogSequenceNumber.Should().Be(committedEvent.EventLogSequenceNumber.Value);
        storedEvent.ExecutionContext.ShouldBeTheSameAs(committedEvent.ExecutionContext);
        storedEvent.Metadata.EventSource.Should().Be(committedEvent.EventSource.Value);
        storedEvent.Metadata.Occurred.Should().Be(committedEvent.Occurred.UtcDateTime);
        storedEvent.Metadata.Public.Should().Be(committedEvent.Public);
        storedEvent.Metadata.TypeId.Should().Be(committedEvent.Type.Id.Value);
        storedEvent.Metadata.TypeGeneration.Should().Be(committedEvent.Type.Generation.Value);
    }
}