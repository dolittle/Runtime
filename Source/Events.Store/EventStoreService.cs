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
            ILogger<EventStoreService> logger)
        {
            _eventStoreFactory = eventStoreFactory;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<grpc.EventCommitResponse> Commit(grpc.UncommittedEvents request, ServerCallContext context)
        {
            _logger.Debug($"Events received : {request.Events.Count}");
            var response = new EventCommitResponse { Reason = string.Empty };
            try
            {
                var events = request.Events.Select(_ => new UncommittedEvent(new Artifact(_.Artifact.Id.To<ArtifactId>(), _.Artifact.Generation), _.Public, _.Content));
                var uncommittedEvents = new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                var committedEvents = await _eventStoreFactory().CommitEvents(uncommittedEvents, context.CancellationToken).ConfigureAwait(false);
                response.Success = true;
                response.Events = committedEvents.ToProtobuf();
                _logger.Information("Events are committed");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Reason = $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}";
                _logger.Error(ex, "Error committing");
            }

            return response;
        }

        /// <inheritdoc/>
        public override async Task<grpc.AggregateEventCommitResponse> CommitForAggregate(grpc.UncommittedAggregateEvents request, ServerCallContext context)
        {
            _logger.Information("Events for Aggregate received");
            var response = new grpc.AggregateEventCommitResponse { Reason = string.Empty };
            try
            {
                var events = request.Events.Select(_ => new UncommittedEvent(new Artifact(_.Artifact.Id.To<ArtifactId>(), _.Artifact.Generation), _.Public, _.Content));
                var eventSourceId = request.EventSource.To<EventSourceId>();
                var aggregateRoot = new Artifact(request.AggregateRoot.To<ArtifactId>(), ArtifactGeneration.First);

                var uncommittedAggregateEvents = new UncommittedAggregateEvents(
                    eventSourceId,
                    aggregateRoot,
                    request.ExpectedAggregateRootVersion,
                    new ReadOnlyCollection<UncommittedEvent>(events.ToList()));

                var committedEvents = await _eventStoreFactory().CommitAggregateEvents(uncommittedAggregateEvents, context.CancellationToken).ConfigureAwait(false);
                response.Events = committedEvents.ToProtobuf();
                response.Success = true;
                _logger.Information("Events for Aggregate committed");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Reason = $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}";
                _logger.Error(ex, "Error committing");
            }

            return response;
        }

        /// <inheritdoc/>
        public override async Task<grpc.FetchForAggregateResponse> FetchForAggregate(grpc.Aggregate request, ServerCallContext context)
        {
            _logger.Debug("Fetch for Aggregate");
            var aggregate = request.AggregateRoot.To<ArtifactId>();
            var eventSource = request.EventSource.To<EventSourceId>();

            var response = new grpc.FetchForAggregateResponse { Reason = string.Empty };
            try
            {
                var committedAggregateEvents = await _eventStoreFactory().FetchForAggregate(eventSource, aggregate, context.CancellationToken).ConfigureAwait(false);
                response.Success = true;
                response.Events = committedAggregateEvents.ToProtobuf();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Reason = $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}";
                _logger.Error(ex, $"Error fetching for aggregate '{aggregate.Value}'");
            }

            return response;
        }
    }
}