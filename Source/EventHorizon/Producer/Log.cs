// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Producer;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Incoming event horizon subscription")]
    internal static partial void IncomingEventHorizonSubscription(ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Incoming event horizon subscription from microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} to tenant: {ProducerTenant} starting at position: {StreamPosition} in partition: {Partition} in stream: {PublicStream}")]
    internal static partial void IncomingEventHorizonSubscriptionWithArguments(
        ILogger logger,
        MicroserviceId consumerMicroservice,
        TenantId consumerTenant,
        TenantId producerTenant,
        StreamPosition streamPosition,
        PartitionId partition,
        StreamId publicStream);

    [LoggerMessage(0, LogLevel.Information, "Microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} successfully subscribed to tenant: {ProducerTenant} starting at position: {StreamPosition} in partition: {Partition} in stream: {PublicStream}")]
    internal static partial void SuccessfullySubscribed(
        ILogger logger,
        MicroserviceId consumerMicroservice,
        TenantId consumerTenant,
        TenantId producerTenant,
        StreamPosition streamPosition,
        PartitionId partition,
        StreamId publicStream);

    [LoggerMessage(0, LogLevel.Warning, "An error occurred in event horizon for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream}")]
    internal static partial void ErrorOccurredInEventHorizon(
        ILogger logger,
        Exception exception,
        MicroserviceId consumerMicroservice,
        TenantId consumerTenant,
        TenantId producerTenant,
        PartitionId partition,
        StreamId publicStream);

    [LoggerMessage(0, LogLevel.Warning, "Event horizon for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream} stopped")]
    internal static partial void EventHorizonStopped(
        ILogger logger,
        MicroserviceId consumerMicroservice,
        TenantId consumerTenant,
        TenantId producerTenant,
        PartitionId partition,
        StreamId publicStream);
    [LoggerMessage(0, LogLevel.Warning, "Event horizon already registered for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream}")]
    internal static partial void EventHorizonAlreadyRegistered(
        ILogger logger,
        MicroserviceId consumerMicroservice,
        TenantId consumerTenant,
        TenantId producerTenant,
        PartitionId partition,
        StreamId publicStream);

    [LoggerMessage(0, LogLevel.Warning, "Disconnecting Event Horizon for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream}")]
    internal static partial void EventHorizonDisconnecting(
        ILogger logger,
        MicroserviceId consumerMicroservice,
        TenantId consumerTenant,
        TenantId producerTenant,
        PartitionId partition,
        StreamId publicStream);
    
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while handling request. FailureId: {FailureId} Reason: {FailureReason}")]
    internal static partial void ErrorOccurredWhileHandlingRequest(ILogger logger, FailureId failureId, string failureReason);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while writing events to event horizon")]
    internal static partial void ErrorWritingEventToEventHorizon(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Trace, "Checking whether Producer Tenant {ProducerTenant} exists")]
    internal static partial void CheckingIfProducerTenantExists(ILogger logger, TenantId producerTenant);
    
    [LoggerMessage(0, LogLevel.Debug, "There are no consents configured for Producer Tenant {ProducerTenant}")]
    internal static partial void NoConsentsConfiguredForProducerTenant(ILogger logger, TenantId producerTenant);
    
    [LoggerMessage(0, LogLevel.Debug, "Producer Tenant {ProducerTenant} is not a configured tenant")]
    internal static partial void ProducerTenantIsNotConfigured(ILogger logger, TenantId producerTenant);
    
    [LoggerMessage(0, LogLevel.Debug, "There are no consent configured for Partition {Partition} in Public Stream {PublicStream} in Tenant {ProducerTenant} to Consumer Tenant {ConsumerTenant} in Microservice {ConsumerMicroservice}")]
    internal static partial void NoConsentsConfiguredForConsumer(
        ILogger logger,
        PartitionId partition,
        StreamId publicStream,
        TenantId producerTenant,
        TenantId consumerTenant,
        MicroserviceId consumerMicroservice);

    [LoggerMessage(0, LogLevel.Debug, "There are multiple consents configured for Partition {Partition} in Public Stream {PublicStream} in Tenant {ProducerTenant} to Consumer Tenant {ConsumerTenant} in Microservice {ConsumerMicroservice}")]
    internal static partial void MultipleConsentsConfiguredForConsumer(
        ILogger logger,
        PartitionId partition,
        StreamId publicStream,
        TenantId producerTenant,
        TenantId consumerTenant,
        MicroserviceId consumerMicroservice);
    
    [LoggerMessage(0, LogLevel.Warning, "Error occurred while creating subscription response")]
    internal static partial void ErrorCreatingSubscriptionResponse(ILogger logger, Exception ex);

    [LoggerMessage(0, LogLevel.Trace, "Checking consents configured for Partition: {Partition} in Public Stream {PublicStream} in Tenant {ProducerTenant} to Consumer Tenant {ConsumerTenant} in Microservice {ConsumerMicroservice}")]
    internal static partial void CheckingConsents(ILogger logger, PartitionId partition, StreamId publicStream, TenantId producerTenant, TenantId consumerTenant, MicroserviceId consumerMicroservice);
}
