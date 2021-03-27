// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the runtime representation of the consumer subscription arguments.
    /// </summary>
    /// <param name="ExecutionContext">The execution context.</param>
    /// <param name="ProjectionDefinition">The projection definition.</param>
    public record ConsumerSubscriptionArguments(
        ExecutionContext ExecutionContext,
        Microservice ConsumerMicroservice,
        TenantId ConsumerTenant,
        TenantId ProducerTenant,
        StreamId PublicStream,
        PartitionId Partition,
        StreamPosition StreamPosition);
}
