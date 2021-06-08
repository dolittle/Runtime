// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessor" />.
    /// </summary>
    public class StreamProcessor : IStreamProcessor
    {
        readonly SubscriptionId _identifier;
        readonly IEventProcessor _eventProcessor;
        readonly IResilientStreamProcessorStateRepository _streamProcessorStates;
        readonly EventsFromEventHorizonFetcher _eventsFetcher;
        readonly IAsyncPolicyFor<ICanFetchEventsFromStream> _eventsFetcherPolicy;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="subscriptionId">The identifier of the subscription the stream processor will receive events for.</param>
        /// <param name="eventProcessor">The event processor that writes the received events to a scoped event log.</param>
        /// <param name="eventsFetcher">The event fetcher that receives fetches events over an event horizon connection.</param>
        /// <param name="streamProcessorStates">The repository to use for getting the subscription state.</param>
        /// <param name="eventsFetcherPolicy">The policy around fetching events.</param>
        /// <param name="loggerFactory">The factory for creating loggers.</param>
        public StreamProcessor(
            SubscriptionId subscriptionId,
            EventProcessor eventProcessor,
            EventsFromEventHorizonFetcher eventsFetcher,
            IResilientStreamProcessorStateRepository streamProcessorStates,
            IAsyncPolicyFor<ICanFetchEventsFromStream> eventsFetcherPolicy,
            ILoggerFactory loggerFactory)
        {
            _identifier = subscriptionId;
            _eventProcessor = eventProcessor;
            _streamProcessorStates = streamProcessorStates;
            _eventsFetcher = eventsFetcher;
            _eventsFetcherPolicy = eventsFetcherPolicy;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<StreamProcessor>();
        }

        /// <inheritdoc/>
        public async Task StartAndWait(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.StreamProcessorCancellationRequested(_identifier);
                return;
            }

            if (_started)
            {
                _logger.StreamProcessorAlreadyProcessingStream(_identifier);
                throw new StreamProcessorAlreadyProcessingStream(_identifier);
            }
            _started = true;

            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(_identifier, cancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                _logger.StreamProcessorPersitingNewState(_identifier);
                tryGetStreamProcessorState = StreamProcessorState.New;
                await _streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.StreamProcessorFetchedState(_identifier, tryGetStreamProcessorState.Result);
            }

            await new ScopedStreamProcessor(
                _identifier.ConsumerTenantId,
                _identifier,
                new StreamDefinition(new FilterDefinition(_identifier.ProducerTenantId.Value, _identifier.ConsumerTenantId.Value, false)),
                tryGetStreamProcessorState.Result as StreamProcessorState,
                _eventProcessor,
                _streamProcessorStates,
                _eventsFetcher,
                _eventsFetcherPolicy,
                _eventsFetcher,
                new TimeToRetryForUnpartitionedStreamProcessor(),
                _loggerFactory.CreateLogger<ScopedStreamProcessor>()).Start(cancellationToken).ConfigureAwait(false);
        }
    }
}
