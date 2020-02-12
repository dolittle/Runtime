// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.EventStore;
using grpc = contracts::Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class EventStoreService : EventStoreBase
    {
        readonly FactoryFor<IEventStore> _eventStoreFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreService"/> class.
        /// </summary>
        /// <param name="eventStoreFactory"><see cref="IEventStore"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventStoreService(
            FactoryFor<IEventStore> eventStoreFactory,
            ILogger logger)
        {
            _eventStoreFactory = eventStoreFactory;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<EventCommitResponse> Commit(grpc.UncommittedEvents request, ServerCallContext context)
        {
            _logger.Debug($"Events received : {request.Events.Count}");
            return await Task.FromResult(new EventCommitResponse()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<EventCommitResponse> CommitForAggregate(grpc.UncommittedAggregateEvents request, ServerCallContext context)
        {
            _logger.Information("Events for Aggregate received");
            try
            {
                var events = request.Events.Select(_ => new UncommittedEvent(new Artifact(_.Artifact.Id.To<ArtifactId>(), _.Artifact.Generation), _.Content));
                var eventSourceId = request.EventSourceId.To<EventSourceId>();
                var aggregateRoot = new Artifact(request.AggregateRoot.To<ArtifactId>(), ArtifactGeneration.First);

                var uncommittedAggregateEvents = new UncommittedAggregateEvents(
                    eventSourceId,
                    aggregateRoot,
                    request.Version,
                    new ReadOnlyCollection<UncommittedEvent>(events.ToList()));

                await _eventStoreFactory().CommitAggregateEvents(uncommittedAggregateEvents).ConfigureAwait(false);
                _logger.Information("Events for Aggregate committed");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error committing");
            }

            return await Task.FromResult(new EventCommitResponse()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<grpc.CommittedAggregateEvents> FetchForAggregate(Aggregate request, ServerCallContext context)
        {
            _logger.Debug("Fetch for Aggregate");
            return await Task.FromResult(new grpc.CommittedAggregateEvents()).ConfigureAwait(false);
        }
    }
}