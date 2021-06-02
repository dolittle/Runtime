// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [SingletonPerTenant]
    public class Subscriptions : ISubscriptions
    {
        readonly ConcurrentDictionary<SubscriptionId, Subscription> _subscriptions = new();
        readonly IResilientStreamProcessorStateRepository _streamProcessorStates;
        readonly IAsyncPolicyFor<ICanFetchEventsFromStream> _eventsFetcherPolicy;
        readonly ILoggerFactory _loggerFactory;
        readonly IStreamProcessorFactory _streamProcessorFactory;
        readonly ILogger<Subscriptions> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public Subscriptions(
            IResilientStreamProcessorStateRepository streamProcessorStates,
            IAsyncPolicyFor<ICanFetchEventsFromStream> eventsFetcherPolicy,
            IStreamProcessorFactory streamProcessorFactory,
            ILoggerFactory loggerFactory)
        {
            _streamProcessorStates = streamProcessorStates;
            _eventsFetcherPolicy = eventsFetcherPolicy;
            _loggerFactory = loggerFactory;
            _streamProcessorFactory = streamProcessorFactory;
            _logger = loggerFactory.CreateLogger<Subscriptions>();
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
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
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
                _loggerFactory,
                cancellationToken);
            if (!_subscriptions.TryAdd(subscriptionId, subscription))
            {
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
                subscription = default;
                return false;
            }

            _logger.SuccessfullyRegisteredSubscription(subscriptionId);
            return true;
        }

        void Unregister(SubscriptionId id)
        {
            _logger.UnregisteringSubscription(id);
            _subscriptions.TryRemove(id, out var _);
        }
    }
}
