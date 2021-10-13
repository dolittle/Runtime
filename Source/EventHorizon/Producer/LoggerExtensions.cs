// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Guid, Guid, ulong, Guid, Guid, Exception> _incomingEventHorizonSubscription = LoggerMessage
            .Define<Guid, Guid, Guid, ulong, Guid, Guid>(
                LogLevel.Debug,
                new EventId(1404825474, nameof(IncomingEventHorizonSubscription)),
                "Incoming event horizon subscription from microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} to tenant: {ProducerTenant} starting at position: {StreamPosition} in partition: {Partition} in stream: {PublicStream}");

        static readonly Action<ILogger, Guid, Guid, Guid, ulong, Guid, Guid, Exception> _successfullySubscribed = LoggerMessage
            .Define<Guid, Guid, Guid, ulong, Guid, Guid>(
                LogLevel.Information,
                new EventId(399974883, nameof(SuccessfullySubscribed)),
                "Microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} successfully subscribed to tenant: {ProducerTenant} starting at position: {StreamPosition} in partition: {Partition} in stream: {PublicStream}");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Exception> _errorOccurredInEventHorizon = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Warning,
                new EventId(255540672, nameof(ErrorOccurredInEventHorizon)),
                "An error occurred in event horizon for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream}");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Exception> _eventHorizonStopped = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Warning,
                new EventId(271973941, nameof(EventHorizonStopped)),
                "Event horizon for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream} stopped");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Exception> _eventHorizonDisconnecting = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Warning,
                new EventId(271973941, nameof(EventHorizonDisconnecting)),
                "Disconnecting Event Horizon for microservice: {ConsumerMicroservice} and tenant: {ConsumerTenant} with producer tenant: {ProducerTenant} in partition: {Partition} in stream: {PublicStream}");


        internal static void IncomingEventHorizonSubscription(
            this ILogger logger,
            Microservice consumerMicroservice,
            TenantId consumerTenant,
            TenantId producerTenant,
            StreamPosition position,
            PartitionId partition,
            StreamId publicStream)
            => _incomingEventHorizonSubscription(logger, consumerMicroservice, consumerTenant, producerTenant, position, partition, publicStream, null);

        internal static void SuccessfullySubscribed(
            this ILogger logger,
            Microservice consumerMicroservice,
            TenantId consumerTenant,
            TenantId producerTenant,
            StreamPosition position,
            PartitionId partition,
            StreamId publicStream)
            => _successfullySubscribed(logger, consumerMicroservice, consumerTenant, producerTenant, position, partition, publicStream, null);

        internal static void ErrorOccurredInEventHorizon(
            this ILogger logger,
            Exception exception,
            Microservice consumerMicroservice,
            TenantId consumerTenant,
            TenantId producerTenant,
            PartitionId partition,
            StreamId publicStream)
            => _errorOccurredInEventHorizon(logger, consumerMicroservice, consumerTenant, producerTenant, partition, publicStream, exception);

        internal static void EventHorizonStopped(
            this ILogger logger,
            Microservice consumerMicroservice,
            TenantId consumerTenant,
            TenantId producerTenant,
            PartitionId partition,
            StreamId publicStream)
            => _eventHorizonStopped(logger, consumerMicroservice, consumerTenant, producerTenant, partition, publicStream, null);

        internal static void EventHorizonDisconnecting(
            this ILogger logger,
            Microservice consumerMicroservice,
            TenantId consumerTenant,
            TenantId producerTenant,
            PartitionId partition,
            StreamId publicStream)
            => _eventHorizonDisconnecting(logger, consumerMicroservice, consumerTenant, producerTenant, partition, publicStream, null);
    }
}