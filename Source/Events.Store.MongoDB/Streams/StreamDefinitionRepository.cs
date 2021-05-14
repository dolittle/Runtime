// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamDefinitionRepository" />.
    /// </summary>
    public class StreamDefinitionRepository : IStreamDefinitionRepository
    {
        readonly IStreams _streams;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinitionRepository"/> class.
        /// </summary>
        /// <param name="streams">The <see cref="IStreams" />.</param>
        public StreamDefinitionRepository(IStreams streams)
        {
            _streams = streams;
        }

        /// <inheritdoc/>
        public async Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            try
            {
                var streamDefinitions = await _streams.GetDefinitions(scope, cancellationToken).ConfigureAwait(false);
                await streamDefinitions.ReplaceOneAsync(
                    Builders<StreamDefinition>.Filter.Eq(_ => _.StreamId, streamDefinition.StreamId.Value),
                    new StreamDefinition(
                        streamDefinition.StreamId,
                        streamDefinition.FilterDefinition.ToStoreRepresentation(),
                        streamDefinition.Partitioned,
                        streamDefinition.Public),
                    new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IStreamDefinition>> TryGet(ScopeId scope, StreamId stream, CancellationToken cancellationToken)
        {
            try
            {
                var streamDefinitions = await _streams.GetDefinitions(scope, cancellationToken).ConfigureAwait(false);
                var streamDefinition = await streamDefinitions.Find(
                    Builders<StreamDefinition>.Filter.Eq(_ => _.StreamId, stream.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return streamDefinition == default
                    ? new StreamDefinitionDoesNotExist(stream, scope)
                    : Try<IStreamDefinition>.Succeeded(streamDefinition.AsRuntimeRepresentation());
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}