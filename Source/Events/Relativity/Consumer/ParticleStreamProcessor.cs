// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents a processor of events in the consumer.
    /// </summary>
    public class ParticleStreamProcessor
    {
        readonly FactoryFor<IEventStore> _getEventStore;
        readonly FactoryFor<IGeodesics> _getGeodesics;
        readonly IScopedEventProcessingHub _processingHub;
        readonly ILogger _logger;
        readonly EventHorizonKey _eventHorizonKey;
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleStreamProcessor"/> class.
        /// </summary>
        /// <param name="getEventStore"><see cref="FactoryFor{IEventStore}" /> factory function that returns a correctly scoped <see cref="IEventStore" />.</param>
        /// <param name="getGeodesics"><see cref="FactoryFor{IGeodesics}" /> factory function that returns a correctly scoped <see cref="IGeodesics" />.</param>
        /// <param name="key">The <see cref="EventHorizonKey" /> to identify the Event Horizon.</param>
        /// <param name="processingHub"><see cref="IScopedEventProcessingHub" /> for processing events from the <see cref="CommittedEventStream" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with <see cref="ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger" /> for logging.</param>
        public ParticleStreamProcessor(
            FactoryFor<IEventStore> getEventStore,
            FactoryFor<IGeodesics> getGeodesics,
            EventHorizonKey key,
            IScopedEventProcessingHub processingHub,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _getEventStore = getEventStore;
            _getGeodesics = getGeodesics;
            _processingHub = processingHub;
            _eventHorizonKey = key;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <summary>
        /// Processes <see cref="CommittedEventStreamWithContext"/>.
        /// </summary>
        /// <param name="committedEventStreamWithContext"><see cref="CommittedEventStreamWithContext"/> to process.</param>
        /// <returns>A <see cref="Task"/> holding <see cref="CommitSequenceNumber"/> representing the asynchronous operation.</returns>
        public Task<CommitSequenceNumber> Process(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            try
            {
                var originatingSequence = committedEventStreamWithContext.EventStream.Sequence;
                var context = committedEventStreamWithContext.Context;
                var particleStream = committedEventStreamWithContext.EventStream;
                EventSourceVersion version = null;

                _executionContextManager.CurrentFor(context);
                using (var eventStore = _getEventStore())
                {
                    version = eventStore.GetNextVersionFor(particleStream.Source.Key);
                }

                var versionedEventSource = new VersionedEventSource(version, new EventSourceKey(particleStream.Source.EventSource, particleStream.Source.Artifact));

                var eventEnvelopes = new List<EventEnvelope>();

                particleStream.Events.ForEach(_ =>
                {
                    _.Metadata.OriginalContext.CommitInOrigin = particleStream.Sequence;
                    var envelope = new EventEnvelope(
                        new EventMetadata(
                            _.Id,
                            new VersionedEventSource(version, new EventSourceKey(particleStream.Source.EventSource, particleStream.Source.Artifact)),
                            _.Metadata.CorrelationId,
                            _.Metadata.Artifact,
                            _.Metadata.Occurred,
                            _.Metadata.OriginalContext),
                        _.Event);

                    eventEnvelopes.Add(envelope);

                    version = version.NextSequence();
                });

                var uncommittedEventStream = new UncommittedEventStream(
                    particleStream.Id,
                    particleStream.CorrelationId,
                    versionedEventSource,
                    particleStream.Timestamp,
                    new EventStream(eventEnvelopes));

                _logger.Information("Commit events to store");
                CommittedEventStream committedEventStream = null;
                using (var eventStore = _getEventStore())
                {
                    committedEventStream = eventStore.Commit(uncommittedEventStream);
                }

                SetOffset(_eventHorizonKey, originatingSequence);
                _logger.Information("Process committed events");
                _processingHub.Process(committedEventStream);
                return Task.FromResult(committedEventStream.Sequence);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Couldn't handle incoming commit");
                return Task.FromException<CommitSequenceNumber>(ex);
            }
        }

        void SetOffset(EventHorizonKey key, CommitSequenceNumber commitSequenceNumber)
        {
            // the provider should accept the version and is responsible for transient errors and persisting it eventually
            try
            {
                using (var repository = _getGeodesics())
                {
                    repository.SetOffset(_eventHorizonKey, commitSequenceNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error setting offset for '{key}' : '{commitSequenceNumber}' - {ex}");
            }
        }
    }
}