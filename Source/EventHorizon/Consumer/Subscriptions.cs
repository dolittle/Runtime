// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [SingletonPerTenant]
    public class Subscriptions : ISubscriptions
    {
        readonly ConcurrentDictionary<SubscriptionId, Subscription> _subscriptions = new ConcurrentDictionary<SubscriptionId, Subscription>();
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly ILoggerManager _loggerManager;
        readonly ILogger<Subscriptions> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        public Subscriptions(IStreamProcessorStateRepository streamProcessorStates, ILoggerManager loggerManager)
        {
            _streamProcessorStates = streamProcessorStates;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<Subscriptions>();
        }

        /// <inheritdoc/>
        public bool HasSubscription(SubscriptionId subscriptionId) => _subscriptions.ContainsKey(subscriptionId);

        /// <inheritdoc />
        public bool TrySubscribe(SubscriptionId subscriptionId, EventProcessor eventProcessor, EventsFromEventHorizonFetcher eventsFetcher, CancellationToken cancellationToken, out Subscription subscription)
        {
            subscription = default;
            try
            {
                if (!_subscriptions.ContainsKey(subscriptionId))
                {
                    _logger.Warning("Subscription: '{streamProcessorId}' already registered", subscriptionId);
                    return false;
                }

                subscription = new Subscription(
                    subscriptionId,
                    eventProcessor,
                    eventsFetcher,
                    _streamProcessorStates,
                    () => _subscriptions.TryRemove(subscriptionId, out var _),
                    _loggerManager,
                    cancellationToken);
                if (!_subscriptions.TryAdd(subscriptionId, subscription))
                {
                    _logger.Warning("Stream Processor with Id: '{streamProcessorId}' already registered", subscriptionId);
                    subscription = default;
                    return false;
                }

                _logger.Trace("Stream Processor with Id: '{streamProcessorId}' registered for Tenant: '{tenant}'", subscriptionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to register Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}'", subscriptionId);
                subscription = default;
                return false;
            }
        }
    }
}
