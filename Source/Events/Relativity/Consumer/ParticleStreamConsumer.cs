
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 * --------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticleStreamProcessor
    {
        readonly FactoryFor<IEventStore> _getEventStore;
        readonly FactoryFor<IGeodesics> _getGeodesics;
        readonly IScopedEventProcessingHub _processingHub;
        readonly ILogger _logger;
        readonly EventHorizonKey _eventHorizonKey;

        /// <summary>
        /// Instantiates an instance of ParticleStreamConsumer
        /// </summary>

        /// <param name="getEventStore"><see cref="FactoryFor{IEventStore}" /> factory function that returns a correctly scoped <see cref="IEventStore" /></param>
        /// <param name="getGeodesics"><see cref="FactoryFor{IGeodesics}" /> factory function that returns a correctly scoped <see cref="IGeodesics" /></param>
        /// <param name="key">The <see cref="EventHorizonKey" /> to identify the Event Horizon</param>
        /// <param name="processingHub"><see cref="IScopedEventProcessingHub" /> for processing events from the <see cref="CommittedEventStream" /></param>
        /// <param name="logger"><see cref="ILogger" /> for logging</param>
        public ParticleStreamProcessor(FactoryFor<IEventStore> getEventStore, FactoryFor<IGeodesics> getGeodesics, EventHorizonKey key, IScopedEventProcessingHub processingHub, ILogger logger)
        {
            _getEventStore = getEventStore;
            _getGeodesics = getGeodesics;
            _processingHub = processingHub;
            _logger = logger;
            _eventHorizonKey = key;
        }

        /// <summary>
        /// Processes
        /// </summary>
        /// <param name="particleStream"></param>
        /// <returns></returns>
        public Task<CommitSequenceNumber> Process(CommittedEventStream particleStream)
        {
            try
            {
                EventSourceVersion version = null;
                using(var _ = _getEventStore())
                {
                    version = _.GetNextVersionFor(particleStream.Source.EventSource);
                }

                var versionedEventSource = new VersionedEventSource(version, particleStream.Source.EventSource, particleStream.Source.Artifact);

                var eventEnvelopes = new List<EventEnvelope>();

                particleStream.Events.ForEach(_ => 
                {
                    _.Metadata.OriginalContext.CommitInOrigin = particleStream.Sequence;
                    var envelope = new EventEnvelope(
                        new EventMetadata(
                            _.Id,
                            new VersionedEventSource(version, particleStream.Source.EventSource, particleStream.Source.Artifact),
                            _.Metadata.CorrelationId,
                            _.Metadata.Artifact,
                            _.Metadata.Occurred,
                            _.Metadata.OriginalContext
                        ), 
                        _.Event
                    );
                    eventEnvelopes.Add(envelope);

                    version = version.NextSequence();
                });

                var uncommittedEventStream = new Store.UncommittedEventStream(
                    particleStream.Id,
                    particleStream.CorrelationId,
                    versionedEventSource,
                    particleStream.Timestamp,
                    new Store.EventStream(eventEnvelopes)
                );

                _logger.Information("Commit events to store");
                Store.CommittedEventStream committedEventStream = null;
                using(var _ = _getEventStore())
                {
                    committedEventStream = _.Commit(uncommittedEventStream);
                }
                SetOffset(_eventHorizonKey, committedEventStream.Sequence);
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
            //the provider should accept the version and is responsible for transient errors and persisting it eventually
            try
            {
                using(var repository = _getGeodesics())
                {
                    repository.SetOffset(_eventHorizonKey,commitSequenceNumber);
                } 
            }
            catch(Exception ex)
            {
                _logger.Error($"Error setting offset for '{key}' : '{commitSequenceNumber}' - {ex.ToString()}");
            }
        }
    }
}