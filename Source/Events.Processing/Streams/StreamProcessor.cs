// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system that processes a stream of events.
    /// </summary>
    public class StreamProcessor : IDisposable
    {
        readonly IEventProcessor _processor;
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;
        readonly IStreamProcessorStates _streamProcessorStates;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IStreamProcessors _streamProcessors;
        readonly string _logMessagePrefix;
        readonly CancellationTokenRegistration _cancellationTokenRegistration;
        Task _task;
        bool _disposed;

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
            _cancellationTokenRegistration = _cancellationToken.Register(() => _streamProcessors.Unregister(Identifier));
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
        /// <remarks>This <see cref="StreamProcessorState" /> does not reflect the persisted state until the BeginProcessing.</remarks>
        public StreamProcessorState CurrentState { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts up the <see cref="StreamProcessor "/>.
        /// </summary>
        /// <returns>The stream processing task.</returns>
        public Task Start() => _task ?? (_task = BeginProcessing());

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            _cancellationTokenRegistration.Dispose();
            _disposed = true;
        }

        Task BeginProcessing()
        {
            return _task ?? Task.Run(
                async () =>
                {
                    try
                    {
                        if (ShouldCancel()) return;
                        CurrentState = await _streamProcessorStates.GetStoredStateFor(Identifier, _cancellationToken).ConfigureAwait(false);
                        do
                        {
                            StreamEvent streamEvent = default;
                            while (streamEvent == default && !ShouldCancel())
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

                            if (ShouldCancel()) break;

                            CurrentState = await _streamProcessorStates.ProcessEventAndChangeStateFor(Identifier, _processor, streamEvent, CurrentState, _cancellationToken).ConfigureAwait(false);
                        }
                        while (!ShouldCancel());
                    }
                    catch (Exception ex)
                    {
                        if (!ShouldCancel())
                        {
                            _logger.Error($"{_logMessagePrefix} failed - {ex}");
                        }
                    }
                });
        }

        Task<StreamEvent> FetchNextEventWithPartitionToProcess()
        {
            _logger.Debug($"{_logMessagePrefix} is fetching event at position '{CurrentState.Position}'.");
            return _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, CurrentState.Position, _cancellationToken);
        }

        bool ShouldCancel() => _disposed || _cancellationToken.IsCancellationRequested;
    }
}