// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Processes an individual <see cref="CommittedEventEnvelope" /> for the correct <see cref="TenantId" />.
    /// </summary>
    public class StreamProcessor
    {
        readonly object lockObject = new object();
        readonly TenantId _tenant;
        readonly IEventProcessor _processor;
        readonly ILogger _logger;
        readonly FactoryFor<IFetchUnprocessedStream> _getUnprocessedStream;
        readonly FactoryFor<IStreamProcessorStateAndPositionRepository> _getStateAndPositionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" /> that this processor is scoped to.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="getStateAndPositionRepository">A factory function to return a correctly scoped instance of <see cref="IStreamProcessorStateAndPositionRepository" />.</param>
        /// <param name="getUnprocessedStream">A factory function to return a correctly scoped instance of <see cref="IFetchUnprocessedStream" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        public StreamProcessor(
            TenantId tenant,
            IEventProcessor processor,
            FactoryFor<IStreamProcessorStateAndPositionRepository> getStateAndPositionRepository,
            FactoryFor<IFetchUnprocessedStream> getUnprocessedStream,
            ILogger logger)
        {
            _tenant = tenant;
            _processor = processor;
            _logger = logger;
            _getUnprocessedStream = getUnprocessedStream;
            _getStateAndPositionRepository = getStateAndPositionRepository;
        }

        /// <summary>
        /// Gets the <see cref="StreamId">stream id</see>.
        /// </summary>
        public StreamId StreamId { get; }

        /// <summary>
        /// Gets the unique identifer for the <see cref="IEventProcessor" />.
        /// </summary>
        public EventProcessorId ProcessorId => _processor?.Identifier;

        /// <summary>
        /// Gets a value indicating whether the processor has caught up with committed events.
        /// </summary>
        public bool HasCaughtUp { get; private set; }

        /// <summary>
        /// Gets the current <see cref="StreamProcessorStateAndPosition" />.
        /// </summary>
        public StreamProcessorStateAndPosition CurrentStateAndPosition { get; private set; } = StreamProcessorStateAndPosition.New;

        /// <summary>
        /// Instructs the Processor to Catch up by processing events that have been committed since the last processed event.
        /// </summary>
        public async virtual void CatchUp()
        {
            using (var repository = _getStateAndPositionRepository())
            {
                CurrentStateAndPosition = repository.Get(ProcessorId);
            }

            var stream = _getUnprocessedStream().GetUnprocessedStream(CurrentStateAndPosition.Position);
            do
            {
                var emtpy = await stream.IsEmpty();
                if (!await stream.Is)
                {
                    eventStream.ForEach(e => ProcessEvent(e));
                    eventStream = unprocessedStreamFetcher.GetUnprocessedEvents(Key.Event.Id, LastVersionProcessed);
                }
            }
            while (!stream.IsEmpty()());
            HasCaughtUp = true;
        }

        /// <summary>
        /// Processes a single <see cref="CommittedEventEnvelope" />.
        /// </summary>
        /// <param name="envelope">The <see cref="CommittedEventEnvelope" />to process.</param>
        public virtual void Process(CommittedEventEnvelope envelope)
        {
            lock (lockObject)
            {
                Console.WriteLine($"Event Processor: {_processor.Identifier.Value} Checking if can process: {envelope.Version.Major}.{envelope.Version.Minor}.{envelope.Version.Revision} - {envelope.Metadata.Artifact.Id} : {CanProcess(envelope.Version)}");
                if (CanProcess(envelope.Version))
                {
                    Console.WriteLine($"Event Processor: {_processor.Identifier.Value} Processing: {envelope.Version.Major}.{envelope.Version.Minor}.{envelope.Version.Revision} - {envelope.Metadata.Artifact.Id}");
                    ProcessEvent(envelope);
                }
            }
        }

        /// <summary>
        /// Indicates whether the processor can process the <see cref="CommittedEventVersion" /> provided.
        /// </summary>
        /// <param name="version"><see cref="CommittedEventVersion"/> to check for.</param>
        /// <returns>True if the processor has caught up and the version is greater than the last processed version, otherwise false.</returns>
        public bool CanProcess(CommittedEventVersion version)
        {
            return HasCaughtUp && version > LastVersionProcessed;
        }

        void ProcessEvent(CommittedEventEnvelope envelope)
        {
            lock (lockObject)
            {
                _logger.Debug($"Processing {envelope.Version} of {Key.Event.Id}");
                var previousVersion = LastVersionProcessed;
                try
                {
                    SetCurrentVersion(envelope.Version);
                    _processor.Process(envelope);
                    _logger.Debug($"Processed {envelope.Version} of {Key.Event.Id}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error Processing {envelope.Version} of {Key.Event.Id}.  Resetting to previous version. {ex.Message}");
                    SetCurrentVersion(previousVersion);
                    throw;
                }
            }

            _logger.Debug($"Processing {envelope.Version} of {Key.Event.Id}");
        }

        void SetCurrentVersion(CommittedEventVersion committedEventVersion)
        {
            // the provider should accept the version and is responsible for transient errors and persisting it eventually
            try
            {
                LastVersionProcessed = committedEventVersion;
                using (var repository = _getStateAndPositionRepository())
                {
                    repository.Set(ProcessorId, LastVersionProcessed);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error setting offset for '{ProcessorId}' : '{committedEventVersion}' - {ex}");
            }
        }
    }
}