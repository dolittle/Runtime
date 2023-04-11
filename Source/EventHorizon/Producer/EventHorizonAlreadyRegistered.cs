// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// The <see cref="Failure"/> that is returned when attempting to register an <see cref="EventHandler"/> that has already been registered.
/// </summary>
/// <param name="eventHorizonId">The event horizon identifier.</param>
public record EventHorizonAlreadyRegistered(EventHorizonId eventHorizonId) : Failure(
    SubscriptionFailures.EventHorizonAlreadyRegistered,
    $"Event horizon already registered for microservice: {eventHorizonId.ConsumerMicroservice} and tenant: {eventHorizonId.ConsumerTenant} with producer tenant: {eventHorizonId.ProducerTenant} in partition: {eventHorizonId.Partition} in stream: {eventHorizonId.PublicStream}"
);
