// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventCommitter" />.
    /// </summary>
    public class EventCommitter : IEventCommitter
    {
        readonly IStreams _streams;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCommitter"/> class.
        /// </summary>
        /// <param name="streams">The <see cref="IStreams" />.</param>
        public EventCommitter(IStreams streams)
        {
            _streams = streams;
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> CommitEvent(
            IClientSessionHandle transaction,
            EventLogSequenceNumber sequenceNumber,
            DateTimeOffset occurred,
            Dolittle.Execution.ExecutionContext executionContext,
            UncommittedEvent @event,
            CancellationToken cancellationToken)
        {
            var eventSource = EventSourceId.NotSet;

            await InsertEvent(
                transaction,
                sequenceNumber,
                occurred,
                eventSource,
                @event,
                new AggregateMetadata(),
                executionContext,
                cancellationToken).ConfigureAwait(false);

            return new CommittedEvent(
                sequenceNumber,
                occurred,
                eventSource,
                executionContext,
                @event.Type,
                @event.Public,
                @event.Content);
        }

        /// <inheritdoc/>
        public async Task<CommittedAggregateEvent> CommitAggregateEvent(
            IClientSessionHandle transaction,
            Artifact aggregateRoot,
            AggregateRootVersion aggregateRootVersion,
            EventLogSequenceNumber version,
            DateTimeOffset occurred,
            EventSourceId eventSource,
            Execution.ExecutionContext executionContext,
            UncommittedEvent @event,
            CancellationToken cancellationToken)
        {
            await InsertEvent(
                transaction,
                version,
                occurred,
                eventSource,
                @event,
                new AggregateMetadata
                {
                    WasAppliedByAggregate = true,
                    TypeId = aggregateRoot.Id,
                    TypeGeneration = aggregateRoot.Generation,
                    Version = aggregateRootVersion
                },
                executionContext,
                cancellationToken).ConfigureAwait(false);
            return new CommittedAggregateEvent(
                aggregateRoot,
                aggregateRootVersion,
                version,
                occurred,
                eventSource,
                executionContext,
                @event.Type,
                @event.Public,
                @event.Content);
        }

        Task InsertEvent(
            IClientSessionHandle transaction,
            EventLogSequenceNumber version,
            DateTimeOffset occurred,
            EventSourceId eventSource,
            UncommittedEvent @event,
            AggregateMetadata aggregate,
            Execution.ExecutionContext executionContext,
            CancellationToken cancellationToken)
        {
            return _streams.DefaultEventLog.InsertOneAsync(
                transaction,
                new Event(
                    version,
                    executionContext.ToStoreRepresentation(),
                    new EventMetadata(
                        occurred,
                        eventSource,
                        @event.Type.Id,
                        @event.Type.Generation,
                        @event.Public,
                        false,
                        version),
                    aggregate,
                    BsonDocument.Parse(@event.Content)),
                cancellationToken: cancellationToken);
        }
    }
}
