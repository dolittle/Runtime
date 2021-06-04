// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Resilience;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonConnection" />.
    /// </summary>
    public class EventHorizonConnection : IEventHorizonConnection
    {
        readonly SubscriptionId _subscription;
        readonly MicroserviceAddress _connectionAddress;
        readonly Contracts.Consumer.ConsumerClient _client;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IEventHorizons _eventHorizons;
        readonly AsyncProducerConsumerQueue<StreamEvent> _eventsFromEventHorizon;
        readonly ILogger _logger;
        readonly CancellationTokenSource _cancellationTokenSource = new();
        readonly TaskCompletionSource<SubscriptionResponse> _firstSubscriptionResponse = new(TaskCreationOptions.RunContinuationsAsynchronously);
        bool _is_first_connection = true;
        bool disposed;

        public EventHorizonConnection(
            SubscriptionId subscription,
            MicroserviceAddress connectionAddress,
            Contracts.Consumer.ConsumerClient client,
            IAsyncPolicyFor<ConsumerClient> policy,
            IStreamProcessorStateRepository streamProcessorStates,
            IEventHorizons eventHorizons,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            ILogger logger)
        {
            _subscription = subscription;
            _connectionAddress = connectionAddress;
            _client = client;
            _policy = policy;
            _streamProcessorStates = streamProcessorStates;
            _eventHorizons = eventHorizons;
            _eventsFromEventHorizon = eventsFromEventHorizon;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<SubscriptionResponse> FirstSubscriptionResponse => _firstSubscriptionResponse.Task;

        /// <inheritdoc/>
        public void StartResilientConnection()
            => _policy.Execute(StartConnection, _cancellationTokenSource.Token).ConfigureAwait(false);

        async Task StartConnection(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Getting an Event Horizon for subscription: {Subcription}", _subscription);
                var eventHorizon = SetupEventHorizon(cancellationToken);
                var response = await Connect(eventHorizon, cancellationToken).ConfigureAwait(false);

                if (_is_first_connection)
                {
                    _firstSubscriptionResponse.SetResult(response);
                    _is_first_connection = false;
                    if (!response.Success)
                    {
                        return;
                    }
                }

                await eventHorizon.StartHandleEvents().ConfigureAwait(false);
                throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
            }
            catch (Exception ex)
            {
                if (_is_first_connection)
                {
                    _firstSubscriptionResponse.SetResult(SubscriptionResponse.Failed(new Protobuf.Failure(SubscriptionFailures.CouldNotConnectToProducerRuntime, $"Connecting to producer runtime failed with exception: {ex.Message}")));
                    return;
                }

                throw;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _firstSubscriptionResponse.TrySetCanceled();
            }

            disposed = true;
        }

        IEventHorizonProcessor SetupEventHorizon(CancellationToken cancellationToken)
            => _eventHorizons.Get(
                _connectionAddress,
                _subscription,
                _eventsFromEventHorizon,
                cancellationToken);

        async Task<SubscriptionResponse> Connect(IEventHorizonProcessor eventHorizon, CancellationToken cancellationToken)
        {
            _logger.TenantSubscribedTo(
                _subscription.ConsumerTenantId,
                _subscription.ProducerTenantId,
                _subscription.ProducerMicroserviceId,
                _connectionAddress);
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(_subscription, cancellationToken).ConfigureAwait(false);

            var publicEventsPosition = tryGetStreamProcessorState.Result?.Position ?? StreamPosition.Start;
            return await eventHorizon.Connect(publicEventsPosition).ConfigureAwait(false);
        }
    }
}
