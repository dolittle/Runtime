// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.EventHorizon.Configuration;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Dolittle.Runtime.Tenancy;
using Grpc.Core;
using Microsoft.Extensions.Options;
using static Dolittle.Runtime.EventHorizon.Contracts.Consumer;
using ProtobufContracts = Dolittle.Protobuf.Contracts;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents the implementation of <see cref="ConsumerBase"/>.
/// </summary>
[PublicService]
public class ConsumerService : ConsumerBase
{
    readonly Func<TenantId, IOptions<EventHorizonsPerMicroserviceConfiguration>> _getEventHorizonsConfiguration;
    readonly ITenants _tenants;
    readonly IInitiateReverseCallServices _reverseCalls;
    readonly IConsumerProtocol _protocol;
    readonly IEventHorizons _eventHorizons;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumerService"/> class.
    /// </summary>
    /// <param name="getEventHorizonsConfiguration">The <see cref="Func{TResult}"/> callback for getting <see cref="EventHorizonsPerMicroserviceConfiguration"/> tenant specific configuration.</param>
    /// <param name="tenants">The <see cref="ITenants"/> system.</param>
    /// <param name="reverseCalls">The <see cref="IInitiateReverseCallServices" />.</param>
    /// <param name="protocol">The <see cref="IConsumerProtocol" />.</param>
    /// <param name="eventHorizons">The <see cref="IEventHorizons"/>.</param>
    /// <param name="metrics">The system for capturing metrics.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ConsumerService(
        Func<TenantId, IOptions<EventHorizonsPerMicroserviceConfiguration>> getEventHorizonsConfiguration,
        ITenants tenants,
        IInitiateReverseCallServices reverseCalls,
        IConsumerProtocol protocol,
        IEventHorizons eventHorizons,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _getEventHorizonsConfiguration = getEventHorizonsConfiguration;
        _tenants = tenants;
        _reverseCalls = reverseCalls;
        _protocol = protocol;
        _eventHorizons = eventHorizons;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task Subscribe(
        IAsyncStreamReader<EventHorizonConsumerToProducerMessage> producerStream,
        IServerStreamWriter<EventHorizonProducerToConsumerMessage> consumerStream,
        ServerCallContext context)
    {
        Log.IncomingEventHorizonSubscription(_logger);
        var connectResult = await _reverseCalls.Connect(
                producerStream,
                consumerStream,
                context,
                _protocol,
                context.CancellationToken,
                true).ConfigureAwait(false);
        if (!connectResult.Success)
        {
            return;
        }

        using var dispatcher = connectResult.Result.dispatcher;
        var arguments = connectResult.Result.arguments;
        _metrics.IncrementTotalIncomingSubscriptions();
        Log.IncomingEventHorizonSubscriptionWithArguments(
            _logger,
            arguments.ConsumerMicroservice,
            arguments.ConsumerTenant,
            arguments.ProducerTenant,
            arguments.StreamPosition,
            arguments.Partition,
            arguments.PublicStream);

        if (!TryGetConsent(arguments, out var consent, out var failureResponse))
        {
            _metrics.IncrementTotalRejectedSubscriptions();
            await dispatcher.Reject(failureResponse, context.CancellationToken).ConfigureAwait(false);
            return;
        }

        _metrics.IncrementTotalAcceptedSubscriptions();
        Log.SuccessfullySubscribed(
            _logger,
            arguments.ConsumerMicroservice,
            arguments.ConsumerTenant,
            arguments.ProducerTenant,
            arguments.StreamPosition,
            arguments.Partition,
            arguments.PublicStream);
        await _eventHorizons.Start(dispatcher, arguments, consent, context.CancellationToken).ConfigureAwait(false);
    }

    bool TryGetConsent(ConsumerSubscriptionArguments arguments, out ConsentId consent, out SubscriptionResponse failureResponse)
    {
        consent = default;
        try
        {
            return TryGetEventHorizonsConfiguration(arguments, out var eventHorizonsConfiguration, out failureResponse)
                && TryGetConsentFromConfiguration(arguments, eventHorizonsConfiguration, out consent, out failureResponse);
        }
        catch (Exception ex)
        {
            Log.ErrorCreatingSubscriptionResponse(_logger, ex);
            failureResponse = new SubscriptionResponse
            {
                Failure = new ProtobufContracts.Failure
                {
                    Id = FailureId.Other.ToProtobuf(),
                    Reason = "Error occurred while creating subscription response"
                }
            };
            return false;
        }
    }

    bool TryGetEventHorizonsConfiguration(ConsumerSubscriptionArguments arguments, out EventHorizonsPerMicroserviceConfiguration eventHorizonsConfiguration, out SubscriptionResponse failureResponse)
    {
        eventHorizonsConfiguration = default;
        failureResponse = default;
        try
        {
            Log.CheckingIfProducerTenantExists(_logger, arguments.ProducerTenant);
            if (!ProducerTenantExists(arguments.ProducerTenant))
            {
                Log.ProducerTenantIsNotConfigured(_logger, arguments.ProducerTenant);
                failureResponse = new SubscriptionResponse
                { 
                    Failure = new ProtobufContracts.Failure
                    {
                        Id = SubscriptionFailures.MissingConsent.ToProtobuf(),
                        Reason = $"There are no consents configured for Producer Tenant {arguments.ProducerTenant}",
                    }
                };
                return false;
            }
            eventHorizonsConfiguration = _getEventHorizonsConfiguration(arguments.ProducerTenant).Value;
            return true;
        }
        catch (CannotParseConfiguration ex)
        {
            Log.NoConsentsConfiguredForProducerTenant(_logger, arguments.ProducerTenant);
            failureResponse = new SubscriptionResponse
            { 
                Failure = new ProtobufContracts.Failure
                {
                    Id = SubscriptionFailures.MissingConsent.ToProtobuf(),
                    Reason = $"There are no consents configured for Producer Tenant {arguments.ProducerTenant}. {ex.Message}",
                }
            };
            return false;
        }
    }
    bool TryGetConsentFromConfiguration(
        ConsumerSubscriptionArguments arguments,
        EventHorizonsPerMicroserviceConfiguration eventHorizonsConfiguration,
        out ConsentId consentId,
        out SubscriptionResponse failureResponse)
    {
        consentId = default;
        failureResponse = default;
        Log.CheckingConsents(
            _logger,
            arguments.Partition,
            arguments.PublicStream,
            arguments.ProducerTenant,
            arguments.ConsumerTenant,
            arguments.ConsumerMicroservice);

        if (!eventHorizonsConfiguration.TryGetValue(arguments.ConsumerMicroservice, out var eventHorizonConfiguration))
        {
            failureResponse = CreateNoConsentsConfiguredResponse(
                arguments,
                $"There are no consents configured for Consumer Microservice {arguments.ConsumerMicroservice}");
            return false;
        }
        foreach (var consent in eventHorizonConfiguration.Consents)
        {
            if (consent.ConsumerTenant == arguments.ConsumerTenant.Value && consent.Stream == arguments.PublicStream.Value && consent.Partition == arguments.Partition.Value)
            {
                consentId = consent.Consent;
                return true;
            }
        }
        failureResponse = CreateNoConsentsConfiguredResponse(
            arguments,
            $"There are no consents configured for Consumer Tenant {arguments.ConsumerTenant} from Public Stream {arguments.PublicStream} and Partition {arguments.Partition}");
        return false;
    }

    SubscriptionResponse CreateNoConsentsConfiguredResponse(ConsumerSubscriptionArguments arguments, FailureReason reason)
    {
        Log.NoConsentsConfiguredForConsumer(
            _logger,
            arguments.Partition,
            arguments.PublicStream,
            arguments.ProducerTenant,
            arguments.ConsumerTenant,
            arguments.ConsumerMicroservice);

        return new SubscriptionResponse
        {
            Failure = new ProtobufContracts.Failure
            {
                Id = SubscriptionFailures.MissingConsent.ToProtobuf(),
                Reason = reason
            }
        };
    }

    bool ProducerTenantExists(TenantId producerTenant) =>
        _tenants.All.Contains(producerTenant);
}
