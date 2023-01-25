// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents an the unique identifier of an Event Horizon Subscription.
/// </summary>
public record SubscriptionId(
    TenantId ConsumerTenantId,
    MicroserviceId ProducerMicroserviceId,
    TenantId ProducerTenantId,
    ScopeId ScopeId,
    StreamId StreamId,
    PartitionId PartitionId) : IStreamProcessorId
{
    /// <inheritdoc/>
    public override string ToString() => $"Consumer Tenant: {ConsumerTenantId.Value} Producer Microservice: {ProducerMicroserviceId.Value} Producer Tenant: {ProducerTenantId.Value} Scope: {ScopeId.Value} Stream: {StreamId.Value} Partition: {PartitionId.Value}'";

    public StreamProcessorKey ToProtobuf()
        => throw new System.NotImplementedException();

    public IStreamProcessorId FromProtobuf(StreamProcessorKey streamProcessorKey)
        => throw new System.NotImplementedException();
}
