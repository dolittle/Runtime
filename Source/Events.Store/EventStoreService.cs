// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventStore;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class EventStoreService : EventStoreBase
    {
        readonly FactoryFor<IEventStore> _eventStoreFactory;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreService"/> class.
        /// </summary>
        /// <param name="eventStoreFactory"><see cref="IEventStore"/>.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventStoreService(
            FactoryFor<IEventStore> eventStoreFactory,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _eventStoreFactory = eventStoreFactory;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<Contracts.CommitEventsResponse> Commit(Contracts.CommitEventsRequest request, ServerCallContext context)
        {
            _executionContextManager.CurrentFor(request.CallContext.ExecutionContext);
            var response = new Contracts.CommitEventsResponse();
            try
            {
                _logger.EventsReceivedForCommitting(false, request.Events.Count);
                var events = request.Events.Select(_ => new UncommittedEvent(_.EventSourceId.ToGuid(), new Artifact(_.Artifact.Id.ToGuid(), _.Artifact.Generation), _.Public, _.Content));
                var uncommittedEvents = new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                var committedEvents = await _eventStoreFactory().CommitEvents(uncommittedEvents, context.CancellationToken).ConfigureAwait(false);
                _logger.LogDebug("Events were successfully committed");
                response.Events.AddRange(committedEvents.ToProtobuf());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error committing events");
                response.Failure = GetFailureFromException(ex);
            }

            return response;
        }

        /// <inheritdoc/>
        public override async Task<Contracts.CommitAggregateEventsResponse> CommitForAggregate(Contracts.CommitAggregateEventsRequest request, ServerCallContext context)
        {
            _executionContextManager.CurrentFor(request.CallContext.ExecutionContext);
            var response = new Contracts.CommitAggregateEventsResponse();
            try
            {
                _logger.EventsReceivedForCommitting(true, request.Events.Events.Count);
                EventSourceId eventSourceId = request.Events.EventSourceId.ToGuid();
                var events = request.Events.Events.Select(_ => new UncommittedEvent(eventSourceId, new Artifact(_.Artifact.Id.ToGuid(), _.Artifact.Generation), _.Public, _.Content));
                var aggregateRoot = new Artifact(request.Events.AggregateRootId.ToGuid (), ArtifactGeneration.First);

                var uncommittedAggregateEvents = new UncommittedAggregateEvents(
                    eventSourceId,
                    aggregateRoot,
                    request.Events.ExpectedAggregateRootVersion,
                    new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                var committedEventsResult = await _eventStoreFactory().CommitAggregateEvents(uncommittedAggregateEvents, context.CancellationToken).ConfigureAwait(false);
                _logger.LogDebug("Aggregate events were successfully committed");
                response.Events = committedEventsResult.ToProtobuf();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error committing aggregate events");
                response.Failure = GetFailureFromException(ex);
            }

            return response;
        }

        /// <inheritdoc/>
        public override async Task<Contracts.FetchForAggregateResponse> FetchForAggregate(Contracts.FetchForAggregateRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Fetch for aggregate");
            _executionContextManager.CurrentFor(request.CallContext.ExecutionContext);
            ArtifactId aggregate = request.Aggregate.AggregateRootId.ToGuid();
            EventSourceId eventSource = request.Aggregate.EventSourceId.ToGuid();

            var response = new Contracts.FetchForAggregateResponse();
            try
            {
                var committedEventsResult = await _eventStoreFactory().FetchForAggregate(eventSource, aggregate, context.CancellationToken).ConfigureAwait(false);
                _logger.LogDebug("Successfully fetched events for aggregate");
                response.Events = committedEventsResult.ToProtobuf();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching events from aggregate");
                response.Failure = GetFailureFromException(ex);
            }

            return response;
        }

        Failure GetFailureFromException(Exception ex) =>
            ex switch
                {
                    EventStoreUnavailable e => new Failure(EventStoreFailures.EventStoreUnavailable, e.Message),
                    EventWasAppliedByOtherAggregateRoot e => new Failure(EventStoreFailures.EventAppliedByOtherAggregateRoot, e.Message),
                    EventWasAppliedToOtherEventSource e => new Failure(EventStoreFailures.EventAppliedToOtherEventSource, e.Message),
                    EventStorePersistenceError e => new Failure(EventStoreFailures.EventStorePersistanceError, e.Message),
                    EventStoreConsistencyError e => new Failure(EventStoreFailures.EventStoreConsistencyError, e.Message),
                    EventLogSequenceIsOutOfOrder e => new Failure(EventStoreFailures.EventLogSequenceIsOutOfOrder, e.Message),
                    EventCanNotBeNull e => new Failure(EventStoreFailures.EventCannotBeNull, e.Message),
                    AggregateRootVersionIsOutOfOrder e => new Failure(EventStoreFailures.AggregateRootVersionOutOfOrder, e.Message),
                    AggregateRootConcurrencyConflict e => new Failure(EventStoreFailures.AggregateRootConcurrencyConflict, e.Message),
                    NoEventsToCommit e => new Failure(EventStoreFailures.NoEventsToCommit, e.Message),
                    _ => new Failure(FailureId.Other, $"Error message: {ex.Message}\nStack Trace: {ex.StackTrace}")
                };
    }
}
