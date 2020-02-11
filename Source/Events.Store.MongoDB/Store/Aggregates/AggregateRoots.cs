// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateRoots" />.
    /// </summary>
    public class AggregateRoots : IAggregateRoots
    {
        readonly FilterDefinitionBuilder<AggregateRoot> _filter = Builders<AggregateRoot>.Filter;
        readonly UpdateDefinitionBuilder<AggregateRoot> _update = Builders<AggregateRoot>.Update;

        /// <inheritdoc/>
        public Task IncrementVersionFor(
            IMongoCollection<AggregateRoot> aggregates,
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken = default)
        {
            ThrowIfNextVersionIsNotGreaterThanExpectedVersion(expectedVersion, nextVersion);

            if (expectedVersion == AggregateRootVersion.Initial)
            {
                return WriteFirstAggregateRootDocument(
                    aggregates,
                    transaction,
                    eventSource,
                    aggregateRoot,
                    expectedVersion,
                    nextVersion,
                    cancellationToken);
            }
            else
            {
                return UpdateExistingAggregateRootDocument(
                    aggregates,
                    transaction,
                    eventSource,
                    aggregateRoot,
                    expectedVersion,
                    nextVersion,
                    cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task<AggregateRootVersion> FetchVersionFor(
            IMongoCollection<AggregateRoot> aggregates,
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            CancellationToken cancellationToken = default)
        {
            var eqFilter = _filter.Eq(_ => _.EventSource, eventSource.Value)
                & _filter.Eq(_ => _.AggregateType, aggregateRoot.Value);
            var aggregateDocuments = await aggregates.Find(
                transaction,
                eqFilter).ToListAsync(cancellationToken).ConfigureAwait(false);

            return aggregateDocuments.Count switch
            {
                0 => AggregateRootVersion.Initial,
                1 => aggregateDocuments[0].Version,
                _ => throw new MultipleAggregateInstancesFound(eventSource, aggregateRoot),
            };
        }

        async Task WriteFirstAggregateRootDocument(
            IMongoCollection<AggregateRoot> aggregates,
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await aggregates.InsertOneAsync(
                    transaction,
                    new AggregateRoot
                    {
                        EventSource = eventSource,
                        AggregateType = aggregateRoot,
                        Version = nextVersion,
                    },
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (MongoDuplicateKeyException)
            {
                var currentVersion = await FetchVersionFor(
                    aggregates,
                    transaction,
                    eventSource,
                    aggregateRoot,
                    cancellationToken).ConfigureAwait(false);
                throw new AggregateRootConcurrencyConflict(currentVersion, expectedVersion);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    var currentVersion = await FetchVersionFor(
                        aggregates,
                        transaction,
                        eventSource,
                        aggregateRoot,
                        cancellationToken).ConfigureAwait(false);
                    throw new AggregateRootConcurrencyConflict(currentVersion, expectedVersion);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        var currentVersion = await FetchVersionFor(
                            aggregates,
                            transaction,
                            eventSource,
                            aggregateRoot,
                            cancellationToken).ConfigureAwait(false);
                        throw new AggregateRootConcurrencyConflict(currentVersion, expectedVersion);
                    }
                }

                throw;
            }
        }

        async Task UpdateExistingAggregateRootDocument(
            IMongoCollection<AggregateRoot> aggregates,
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken = default)
        {
            var aggregateRootFilter =
                _filter.Eq(_ => _.EventSource, eventSource.Value)
                    & _filter.Eq(_ => _.AggregateType, aggregateRoot.Value)
                    & _filter.Eq(_ => _.Version, expectedVersion.Value);

            var updateDefinition = _update.Set(_ => _.Version, nextVersion.Value);
            var result = await aggregates.UpdateOneAsync(
                transaction,
                aggregateRootFilter,
                updateDefinition,
                new UpdateOptions { IsUpsert = false },
                cancellationToken).ConfigureAwait(false);

            if (result.ModifiedCount != 0)
            {
                var currentVersion = await FetchVersionFor(
                    aggregates,
                    transaction,
                    eventSource,
                    aggregateRoot,
                    cancellationToken).ConfigureAwait(false);
                throw new AggregateRootConcurrencyConflict(currentVersion, expectedVersion);
            }
        }

        void ThrowIfNextVersionIsNotGreaterThanExpectedVersion(AggregateRootVersion expectedVersion, AggregateRootVersion nextVersion)
        {
            if (nextVersion <= expectedVersion)
            {
                throw new NextAggregateRootVersionMustBeGreaterThanCurrentVersion(expectedVersion, nextVersion);
            }
        }
    }
}