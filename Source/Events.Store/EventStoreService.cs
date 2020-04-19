// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Contracts.EventStore;
using grpc = contracts::Dolittle.Runtime.Events.Contracts;

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
        public override async Task<grpc.CommitEventsResponse> Commit(grpc.CommitEventsRequest request, ServerCallContext context)
        {
            var response = new grpc.CommitEventsResponse();
            try
            {
                _logger.Debug("{eventsCount} Events received for committing", request.Events.Count);
                var events = request.Events.Select(_ => new UncommittedEvent(new Artifact(_.Artifact.Id.To<ArtifactId>(), _.Artifact.Generation), _.Public, _.Content));
                var uncommittedEvents = new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                var committedEventsResult = await _eventStoreFactory().CommitEvents(uncommittedEvents, context.CancellationToken).ConfigureAwait(false);

                if (committedEventsResult.Success)
                {
                    _logger.Debug("Events were successfully committed");
                    response.Events.AddRange(committedEventsResult.Events.ToProtobuf());
                }
                else
                {
                    _logger.Debug("Events were unsuccessfully committed");
                    response.Failure = committedEventsResult.Failure.AsFailure().ToProtobuf();
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error committing events");
                response.Failure = new Failure(FailureId.Other, $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}").ToProtobuf();
            }

            return response;
        }

        /// <inheritdoc/>
        public override async Task<grpc.CommitAggregateEventsResponse> CommitForAggregate(grpc.CommitAggregateEventsRequest request, ServerCallContext context)
        {
            var response = new grpc.CommitAggregateEventsResponse();
            try
            {
                _logger.Debug("{eventsCount} Aggregate Events received for committing", request.Events.Events.Count);
                var events = request.Events.Events.Select(_ => new UncommittedEvent(new Artifact(_.Artifact.Id.To<ArtifactId>(), _.Artifact.Generation), _.Public, _.Content));
                var eventSourceId = request.Events.EventSourceId.To<EventSourceId>();
                var aggregateRoot = new Artifact(request.Events.AggregateRootId.To<ArtifactId>(), ArtifactGeneration.First);

                var uncommittedAggregateEvents = new UncommittedAggregateEvents(
                    eventSourceId,
                    aggregateRoot,
                    request.Events.ExpectedAggregateRootVersion,
                    new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                var committedEventsResult = await _eventStoreFactory().CommitAggregateEvents(uncommittedAggregateEvents, context.CancellationToken).ConfigureAwait(false);

                if (committedEventsResult.Success)
                {
                    _logger.Debug("Aggregate Events were successfully committed");
                    response.Events = committedEventsResult.Events.ToProtobuf();
                }
                else
                {
                    _logger.Debug("Aggregate Events were unsuccessfully committed");
                    response.Failure = committedEventsResult.Failure.AsFailure().ToProtobuf();
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error committing events");
                response.Failure = new Failure(FailureId.Other, $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}").ToProtobuf();
            }

            return response;
        }

        /// <inheritdoc/>
        public override async Task<grpc.FetchForAggregateResponse> FetchForAggregate(grpc.FetchForAggregateRequest request, ServerCallContext context)
        {
            _logger.Debug("Fetch for Aggregate");
            var aggregate = request.Aggregate.AggregateRootId.To<ArtifactId>();
            var eventSource = request.Aggregate.EventSourceId.To<EventSourceId>();

            var response = new grpc.FetchForAggregateResponse();
            try
            {
                var committedEventsResult = await _eventStoreFactory().FetchForAggregate(eventSource, aggregate, context.CancellationToken).ConfigureAwait(false);

                if (committedEventsResult.Success)
                {
                    _logger.Debug("Successfully fetched events for aggregate");
                    response.Events = committedEventsResult.Events.ToProtobuf();
                }
                else
                {
                    _logger.Debug("Unsuccessfully fetched events for aggregate");
                    response.Failure = committedEventsResult.Failure.AsFailure().ToProtobuf();
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error committing events");
                response.Failure = new Failure(FailureId.Other, $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}").ToProtobuf();
            }

            return response;
        }
    }
}