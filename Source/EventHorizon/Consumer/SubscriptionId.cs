// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an the unique identifier of an Event Horizon Subscription.
    /// </summary>
    public record SubscriptionId(
        TenantId ConsumerTenantId,
        Microservice ProducerMicroserviceId,
        TenantId ProducerTenantId,
        ScopeId ScopeId,
        StreamId StreamId,
        PartitionId PartitionId) : IStreamProcessorId
    {
        /// <inheritdoc/>
        public override string ToString() => $"Consumer Tenant: '{ConsumerTenantId} Producer Microservice: '{ProducerMicroserviceId}' Producer Tenant: '{ProducerTenantId}' Scope: '{ScopeId}' Stream: '{StreamId}' Partition: '{PartitionId}''";
    }
}
