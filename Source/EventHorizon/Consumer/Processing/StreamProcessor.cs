// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Rudimentary;

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
        /// <param name="consentId">The <see cref="ConsentId" />.</param>
        /// <param name="subscriptionId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFetcher">The <see cref="EventsFromEventHorizonFetcher" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            ConsentId consentId,
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
            ConsentId = consentId;
        }

        /// <summary>
        /// Gets the <see cref="ConsentId" />.
        /// </summary>
        public ConsentId ConsentId { get; }

        /// <inheritdoc/>
        public async Task<Try<bool>> TryStartAndWait(CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new OperationCanceledException("Won't start event horizon stream processor because operation is cancelled", cancellationToken);
                }
                if (_started)
                {
                    return new StreamProcessorAlreadyProcessingStream(_identifier);
                }
                _started = true;
                var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(_identifier, cancellationToken).ConfigureAwait(false);
                if (!tryGetStreamProcessorState.Success)
                {
                    tryGetStreamProcessorState = StreamProcessorState.New;
                    await _streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, cancellationToken).ConfigureAwait(false);
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

                return new Try<bool>(true, true);
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                _started = false;
            }
        }
    }
}
