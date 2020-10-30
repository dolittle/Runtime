// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
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
        readonly IAggregatesCollection _aggregates;
        readonly ILogger<AggregateRoots> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoots"/> class.
        /// </summary>
        /// <param name="aggregates">The <see cref="IAggregatesCollection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregateRoots(IAggregatesCollection aggregates, ILogger<AggregateRoots> logger)
        {
            _aggregates = aggregates;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<AggregateRoot> IncrementVersionFor(
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken)
        {
            _logger.Trace("Incrementing version for Aggregate: {AggregateRoot} and Event Source Id: {EventSourceId}", aggregateRoot, eventSource);
            ThrowIfNextVersionIsNotGreaterThanExpectedVersion(eventSource, aggregateRoot, expectedVersion, nextVersion);

            if (expectedVersion == AggregateRootVersion.Initial)
            {
                return WriteFirstAggregateRootDocument(
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
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            CancellationToken cancellationToken)
        {
            _logger.Trace("Fetching version version for Aggregate: {AggregateRoot} and Event Source Id: {EventSourceId}", aggregateRoot, eventSource);
            var eqFilter = _filter.Eq(_ => _.EventSource, eventSource.Value)
                & _filter.Eq(_ => _.AggregateType, aggregateRoot.Value);
            var aggregateDocuments = await _aggregates.Aggregates.Find(
                transaction,
                eqFilter).ToListAsync(cancellationToken).ConfigureAwait(false);

            return aggregateDocuments.Count switch
            {
                0 => AggregateRootVersion.Initial,
                1 => aggregateDocuments[0].Version,
                _ => throw new MultipleAggregateInstancesFound(eventSource, aggregateRoot),
            };
        }

        async Task<AggregateRoot> WriteFirstAggregateRootDocument(
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken)
        {
            try
            {
                var aggregateRootDocument = new AggregateRoot(eventSource, aggregateRoot, nextVersion);
                await _aggregates.Aggregates.InsertOneAsync(
                    transaction,
                    aggregateRootDocument,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                return aggregateRootDocument;
            }
            catch (MongoDuplicateKeyException)
            {
                var currentVersion = await FetchVersionFor(
                    eventSource,
                    aggregateRoot,
                    cancellationToken).ConfigureAwait(false);
                throw new AggregateRootConcurrencyConflict(eventSource, aggregateRoot, currentVersion, expectedVersion);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    var currentVersion = await FetchVersionFor(
                        eventSource,
                        aggregateRoot,
                        cancellationToken).ConfigureAwait(false);
                    throw new AggregateRootConcurrencyConflict(eventSource, aggregateRoot, currentVersion, expectedVersion);
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
                            eventSource,
                            aggregateRoot,
                            cancellationToken).ConfigureAwait(false);
                        throw new AggregateRootConcurrencyConflict(eventSource, aggregateRoot, currentVersion, expectedVersion);
                    }
                }

                throw;
            }
        }

        async Task<AggregateRoot> UpdateExistingAggregateRootDocument(
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken)
        {
            var aggregateRootFilter =
                _filter.Eq(_ => _.EventSource, eventSource.Value)
                    & _filter.Eq(_ => _.AggregateType, aggregateRoot.Value)
                    & _filter.Eq(_ => _.Version, expectedVersion.Value);

            var updateDefinition = _update.Set(_ => _.Version, nextVersion.Value);
            var result = await _aggregates.Aggregates.UpdateOneAsync(
                transaction,
                aggregateRootFilter,
                updateDefinition,
                new UpdateOptions { IsUpsert = false },
                cancellationToken).ConfigureAwait(false);

            if (result.ModifiedCount == 0)
            {
                var currentVersion = await FetchVersionFor(
                    transaction,
                    eventSource,
                    aggregateRoot,
                    cancellationToken).ConfigureAwait(false);
                throw new AggregateRootConcurrencyConflict(eventSource, aggregateRoot, currentVersion, expectedVersion);
            }

            if (result.ModifiedCount > 1) throw new MultipleAggregateInstancesFound(eventSource, aggregateRoot);
            return new AggregateRoot(eventSource, aggregateRoot, nextVersion);
        }

        async Task<AggregateRootVersion> FetchVersionFor(
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            CancellationToken cancellationToken)
        {
            var eqFilter = _filter.Eq(_ => _.EventSource, eventSource.Value)
                & _filter.Eq(_ => _.AggregateType, aggregateRoot.Value);
            var aggregateDocuments = await _aggregates.Aggregates.Find(eqFilter).ToListAsync(cancellationToken).ConfigureAwait(false);

            return aggregateDocuments.Count switch
            {
                0 => AggregateRootVersion.Initial,
                1 => aggregateDocuments[0].Version,
                _ => throw new MultipleAggregateInstancesFound(eventSource, aggregateRoot),
            };
        }

        void ThrowIfNextVersionIsNotGreaterThanExpectedVersion(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion expectedVersion, AggregateRootVersion nextVersion)
        {
            if (nextVersion <= expectedVersion)
            {
                throw new NextAggregateRootVersionMustBeGreaterThanCurrentVersion(eventSource, aggregateRoot, expectedVersion, nextVersion);
            }
        }
    }
}