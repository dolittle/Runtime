// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [SingletonPerTenant]
    public class Subscriptions : ISubscriptions
    {
        readonly ConcurrentDictionary<SubscriptionId, Subscription> _subscriptions = new ConcurrentDictionary<SubscriptionId, Subscription>();
        readonly IResilientStreamProcessorStateRepository _streamProcessorStates;
        readonly IAsyncPolicyFor<ICanFetchEventsFromStream> _eventsFetcherPolicy;
        readonly ILoggerManager _loggerManager;
        readonly ILogger<Subscriptions> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        public Subscriptions(IResilientStreamProcessorStateRepository streamProcessorStates, IAsyncPolicyFor<ICanFetchEventsFromStream> eventsFetcherPolicy, ILoggerManager loggerManager)
        {
            _streamProcessorStates = streamProcessorStates;
            _eventsFetcherPolicy = eventsFetcherPolicy;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<Subscriptions>();
        }

        /// <inheritdoc/>
        public bool TryGetConsentFor(SubscriptionId subscriptionId, out ConsentId consentId)
        {
            var result = _subscriptions.TryGetValue(subscriptionId, out var subscription);
            consentId = subscription?.ConsentId;
            return result;
        }

        /// <inheritdoc />
        public bool TrySubscribe(
            ConsentId consentId,
            SubscriptionId subscriptionId,
            EventProcessor eventProcessor,
            EventsFromEventHorizonFetcher eventsFetcher,
            CancellationToken cancellationToken,
            out Subscription subscription)
        {
            subscription = default;
            if (_subscriptions.ContainsKey(subscriptionId))
            {
                _logger.Warning("Subscription: '{SubscriptionId}' already registered", subscriptionId);
                return false;
            }

            subscription = new Subscription(
                consentId,
                subscriptionId,
                eventProcessor,
                eventsFetcher,
                _streamProcessorStates,
                () => Unregister(subscriptionId),
                _eventsFetcherPolicy,
                _loggerManager,
                cancellationToken);
            if (!_subscriptions.TryAdd(subscriptionId, subscription))
            {
                _logger.Warning("Subscription: '{SubscriptionId}' already registered", subscriptionId);
                subscription = default;
                return false;
            }

            _logger.Trace("Subscription: '{SubscriptionId}' registered", subscriptionId);
            return true;
        }

        void Unregister(SubscriptionId id)
        {
            _logger.Debug("Unregistering Subscription: {subscriptionId}", id);
            _subscriptions.TryRemove(id, out var _);
        }
    }
}
