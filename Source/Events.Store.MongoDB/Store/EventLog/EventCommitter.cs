// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Tenancy;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventLog
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventCommitter" />.
    /// </summary>
    public class EventCommitter : IEventCommitter
    {
        /// <inheritdoc/>
        public async Task<CommittedEvent> CommitEvent(
            IMongoCollection<Event> events,
            IClientSessionHandle transaction,
            EventLogVersion version,
            DateTimeOffset occurred,
            Dolittle.Execution.ExecutionContext executionContext,
            Cause cause,
            UncommittedEvent @event,
            CancellationToken cancellationToken = default)
        {
            var correlation = executionContext.CorrelationId;
            Microservice microservice = executionContext.BoundedContext.Value;
            var tenant = executionContext.Tenant;

            await InsertEvent(
                events,
                transaction,
                version,
                occurred,
                @event,
                new AggregateMetadata(),
                correlation,
                microservice,
                tenant,
                cause,
                cancellationToken).ConfigureAwait(false);

            return new CommittedEvent(
                version,
                occurred,
                correlation,
                microservice,
                tenant,
                cause,
                @event.Type,
                @event.Content);
        }

        /// <inheritdoc/>
        public async Task<CommittedAggregateEvent> CommitAggregateEvent(
            IMongoCollection<Event> events,
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            Artifact aggregateRoot,
            AggregateRootVersion aggregateRootVersion,
            EventLogVersion version,
            DateTimeOffset occurred,
            Dolittle.Execution.ExecutionContext executionContext,
            Cause cause,
            UncommittedEvent @event,
            CancellationToken cancellationToken = default)
        {
            var correlation = executionContext.CorrelationId;
            Microservice microservice = executionContext.BoundedContext.Value;
            var tenant = executionContext.Tenant;
            await InsertEvent(
                events,
                transaction,
                version,
                occurred,
                @event,
                new AggregateMetadata
                {
                    WasAppliedByAggregate = true,
                    EventSourceId = eventSource,
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
                eventSource,
                aggregateRoot,
                aggregateRootVersion,
                version,
                occurred,
                correlation,
                microservice,
                tenant,
                cause,
                @event.Type,
                @event.Content);
        }

        async Task InsertEvent(
            IMongoCollection<Event> events,
            IClientSessionHandle transaction,
            EventLogVersion version,
            DateTimeOffset occurred,
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
                await events.InsertOneAsync(
                    transaction,
                    new Event
                    {
                        EventLogVersion = version,
                        Metadata = new EventMetadata
                        {
                            Occurred = occurred,
                            Correlation = correlation,
                            Microservice = microservice,
                            Tenant = tenant,
                            CauseType = cause.Type,
                            CausePosition = cause.Position,
                            TypeId = @event.Type.Id,
                            TypeGeneration = @event.Type.Generation,
                        },
                        Aggregate = aggregate,
                        Content = BsonDocument.Parse(@event.Content),
                    },
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (MongoDuplicateKeyException)
            {
                throw new EventLogDuplicateKeyError(version);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new EventLogDuplicateKeyError(version);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new EventLogDuplicateKeyError(version);
                    }
                }

                throw;
            }
        }
    }
}