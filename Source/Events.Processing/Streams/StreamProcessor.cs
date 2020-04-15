// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

#pragma warning disable CA2008

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system that processes a stream of events.
    /// </summary>
    public class StreamProcessor
    {
        readonly IEventProcessor _processor;
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly string _logMessagePrefix;
        readonly IStreamProcessorStates _streamProcessorStates;
        readonly IStreamProcessors _streamProcessors;
        Task _task;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            IEventProcessor processor,
            IStreamProcessorStates streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            IStreamProcessors streamProcessors,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _processor = processor;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _streamProcessorStates = streamProcessorStates;
            _streamProcessors = streamProcessors;
            _logger = logger;
            _cancellationToken = cancellationToken;
            Identifier = new StreamProcessorId(_processor.Scope, _processor.Identifier, sourceStreamId);
            CurrentState = StreamProcessorState.New;
            _logMessagePrefix = $"Stream Partition Processor for event processor '{Identifier.EventProcessorId}' in scope {Identifier.ScopeId} with source stream '{Identifier.SourceStreamId}' for tenant '{tenantId}'";
        }

        /// <summary>
        /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="StreamProcessor"/>.
        /// </summary>
        public StreamProcessorId Identifier { get; }

        /// <summary>
        /// Gets the <see cref="EventProcessorId" />.
        /// </summary>
        public EventProcessorId EventProcessorId => _processor.Identifier;

        /// <summary>
        /// Gets the current <see cref="StreamProcessorState" />.
        /// </summary>
        public StreamProcessorState CurrentState { get; private set; }

        /// <summary>
        /// Start processing.
        /// </summary>
        public void Start()
        {
            _task = Task.Factory.StartNew(BeginProcessing, TaskCreationOptions.DenyChildAttach);
        }

        /// <summary>
        /// Starts up the <see cref="StreamProcessor" />.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task BeginProcessing()
        {
            try
            {
                CurrentState = await _streamProcessorStates.GetStoredStateFor(Identifier, _cancellationToken).ConfigureAwait(false);
                using var ctr = _cancellationToken.Register(() => _streamProcessors.Unregister(Identifier.ScopeId, Identifier.EventProcessorId, Identifier.SourceStreamId));
                do
                {
                    StreamEvent streamEvent = default;
                    while (streamEvent == default && !_cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            CurrentState = await _streamProcessorStates.FailingPartitions.CatchupFor(Identifier, _processor, CurrentState, _cancellationToken).ConfigureAwait(false);
                            streamEvent = await FetchNextEventWithPartitionToProcess().ConfigureAwait(false);
                        }
                        catch (NoEventInStreamAtPosition)
                        {
                            await Task.Delay(250).ConfigureAwait(false);
                        }
                        catch (EventStoreUnavailable)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);
                        }
                    }

                    if (_cancellationToken.IsCancellationRequested) break;

                    CurrentState = await _streamProcessorStates.ProcessEventAndChangeStateFor(Identifier, _processor, streamEvent, CurrentState, _cancellationToken).ConfigureAwait(false);
                }
                while (!_cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    _logger.Error($"{_logMessagePrefix} failed - {ex}");
                }
            }
        }

        Task<StreamEvent> FetchNextEventWithPartitionToProcess()
        {
            _logger.Debug($"{_logMessagePrefix} is fetching event at position '{CurrentState.Position}'.");
            return _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, CurrentState.Position, _cancellationToken);
        }
    }
}