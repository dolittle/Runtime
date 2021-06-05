// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer
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
        public override string ToString() => $"Consumer Tenant: '{ConsumerTenantId.Value} Producer Microservice: '{ProducerMicroserviceId.Value}' Producer Tenant: '{ProducerTenantId.Value}' Scope: '{ScopeId.Value}' Stream: '{StreamId.Value}' Partition: '{PartitionId.Value}''";
    }
}
