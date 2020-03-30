// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventCommitter" />.
    /// </summary>
    public class EventCommitter : IEventCommitter
    {
        readonly IMongoCollection<Event> _allStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCommitter"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        public EventCommitter(EventStoreConnection connection)
        {
            _allStream = connection.EventLog;
        }

        /// <inheritdoc/>
        public async Task<CommittedEvent> CommitEvent(
            IClientSessionHandle transaction,
            EventLogSequenceNumber sequenceNumber,
            DateTimeOffset occurred,
            Dolittle.Execution.ExecutionContext executionContext,
            Cause cause,
            UncommittedEvent @event,
            CancellationToken cancellationToken = default)
        {
            var correlation = executionContext.CorrelationId;
            var microservice = executionContext.Microservice;
            var tenant = executionContext.Tenant;
            var eventSource = EventSourceId.NotSet;

            await InsertEvent(
                transaction,
                sequenceNumber,
                occurred,
                eventSource,
                @event,
                new AggregateMetadata(),
                correlation,
                microservice,
                tenant,
                cause,
                cancellationToken).ConfigureAwait(false);

            return new CommittedEvent(
                sequenceNumber,
                occurred,
                eventSource,
                correlation,
                microservice,
                tenant,
                cause,
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
            Cause cause,
            UncommittedEvent @event,
            CancellationToken cancellationToken = default)
        {
            var correlation = executionContext.CorrelationId;
            var microservice = executionContext.Microservice;
            var tenant = executionContext.Tenant;
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
                correlation,
                microservice,
                tenant,
                cause,
                cancellationToken).ConfigureAwait(false);
            return new CommittedAggregateEvent(
                aggregateRoot,
                aggregateRootVersion,
                version,
                occurred,
                eventSource,
                correlation,
                microservice,
                tenant,
                cause,
                @event.Type,
                @event.Public,
                @event.Content);
        }

        async Task InsertEvent(
            IClientSessionHandle transaction,
            EventLogSequenceNumber version,
            DateTimeOffset occurred,
            EventSourceId eventSource,
            UncommittedEvent @event,
            AggregateMetadata aggregate,
            CorrelationId correlation,
            Microservice microservice,
            TenantId tenant,
            Cause cause,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _allStream.InsertOneAsync(
                    transaction,
                    new Event(
                        version,
                        new ExecutionContext(
                            correlation,
                            microservice,
                            tenant),
                        new EventMetadata(
                            occurred,
                            eventSource,
                            cause.Type,
                            cause.Position,
                            @event.Type.Id,
                            @event.Type.Generation,
                            @event.Public),
                        aggregate,
                        BsonDocument.Parse(@event.Content)),
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (MongoDuplicateKeyException)
            {
                throw new EventAlreadyWrittenToStream(@event.Type.Id, version, StreamId.AllStreamId, ScopeId.Default);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new EventAlreadyWrittenToStream(@event.Type.Id, version, StreamId.AllStreamId, ScopeId.Default);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new EventAlreadyWrittenToStream(@event.Type.Id, version, StreamId.AllStreamId, ScopeId.Default);
                    }
                }

                throw;
            }
        }
    }
}