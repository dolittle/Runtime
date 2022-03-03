using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents the unique identifier for an event horizon.
/// </summary>
/// <param name="ConsumerMicroservice">The consumer Microservice.</param>
/// <param name="ConsumerTenant">The consumer Tenant.</param>
/// <param name="ProducerTenant">The producer Tenant.</param>
/// <param name="PublicStream">The public stream of events to be transferred over the Event Horizon..</param>
/// <param name="Partition">The partition of the public stream.</param>
public record EventHorizonId(
    MicroserviceId ConsumerMicroservice,
    TenantId ConsumerTenant,
    TenantId ProducerTenant,
    StreamId PublicStream,
    PartitionId Partition);
