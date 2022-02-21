// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.EventHorizon.Consumer.Connections;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Resilience;
using Microservices;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents an implementation of <see cref="ISubscriptionFactory" />.
/// </summary>
[Singleton, PerTenant]
public class SubscriptionFactory : ISubscriptionFactory
{
    readonly ILoggerFactory _loggerFactory;
    readonly IStreamProcessorFactory _streamProcessorFactory;
    readonly IEventHorizonConnectionFactory _eventHorizonConnectionFactory;
    readonly IAsyncPolicyFor<Subscription> _subscriptionPolicy;
    readonly IGetNextEventToReceiveForSubscription _subscriptionPositions;
    readonly IMetricsCollector _metrics;
    readonly Processing.IMetricsCollector _processingMetrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionFactory"/> class.
    /// </summary>
    /// <param name="streamProcessorFactory">The factory to use for creating stream processors that write the received events.</param>
    /// <param name="microservicesConfiguration">The configuration to use for finding the address of a producer Runtime from it's microservice id.</param>
    /// <param name="eventHorizonConnectionFactory">The factory to use for creating new connections to the producer Runtime.</param>
    /// <param name="subscriptionPolicy">The policy to use for handling the <see cref="SubscribeLoop(CancellationToken)"/>.</param>
    /// <param name="subscriptionPositions">The system to use for getting the next event to receive for a subscription.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    /// <param name="processingMetrics">The system for collecting metrics for event horizon consumer processing.</param>
    /// <param name="loggerFactory">The logger factory to use for creating loggers.</param>
    public SubscriptionFactory(
        IStreamProcessorFactory streamProcessorFactory,
        IEventHorizonConnectionFactory eventHorizonConnectionFactory,
        IAsyncPolicyFor<Subscription> subscriptionPolicy,
        IGetNextEventToReceiveForSubscription subscriptionPositions,
        IMetricsCollector metrics,
        Processing.IMetricsCollector processingMetrics,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _streamProcessorFactory = streamProcessorFactory;
        _eventHorizonConnectionFactory = eventHorizonConnectionFactory;
        _subscriptionPolicy = subscriptionPolicy;
        _subscriptionPositions = subscriptionPositions;
        _metrics = metrics;
        _processingMetrics = processingMetrics;
    }
    /// <inheritdoc />
    public ISubscription Create(SubscriptionId subscriptionId, MicroserviceAddress producerMicroserviceAddress)
        => new Subscription(
            subscriptionId,
            producerMicroserviceAddress,
            _subscriptionPolicy,
            _eventHorizonConnectionFactory,
            _streamProcessorFactory,
            _subscriptionPositions,
            _metrics,
            _processingMetrics,
            _loggerFactory.CreateLogger<Subscription>());
}
