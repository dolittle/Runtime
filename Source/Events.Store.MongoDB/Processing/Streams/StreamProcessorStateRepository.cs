// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorStateRepository" />.
    /// </summary>
    public class StreamProcessorStateRepository : IStreamProcessorStateRepository
    {
        readonly FilterDefinitionBuilder<StreamProcessorState> _streamProcessorFilter = Builders<StreamProcessorState>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStateRepository"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public StreamProcessorStateRepository(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Processing.Streams.StreamProcessorState> GetOrAddNew(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default)
        {
            try
            {
                var states = _connection.StreamProcessorStates;
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (state == default)
                {
                    state = StreamProcessorState.NewFromId(streamProcessorId);
                    await states.InsertOneAsync(state, null, cancellationToken).ConfigureAwait(false);
                }

                return state.ToRuntimeRepresentation();
            }
            catch (MongoDuplicateKeyException)
            {
                throw new StreamProcessorKeyAlreadyRegistered(streamProcessorId);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new StreamProcessorKeyAlreadyRegistered(streamProcessorId);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new StreamProcessorKeyAlreadyRegistered(streamProcessorId);
                    }
                }

                throw;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Processing.Streams.StreamProcessorState> IncrementPosition(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default)
        {
            try
            {
                var states = _connection.StreamProcessorStates;
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (state == default) throw new StreamProcessorNotFound(streamProcessorId);

                state.Position++;

                var replaceResult = await states.ReplaceOneAsync(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)),
                    state,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (replaceResult.MatchedCount == 0) throw new StreamProcessorNotFound(streamProcessorId);

                return state.ToRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Processing.Streams.StreamProcessorState> AddFailingPartition(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition position, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId);
                var states = _connection.StreamProcessorStates;
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, id))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (state == default) throw new StreamProcessorNotFound(streamProcessorId);

                if (state.FailingPartitions.ContainsKey(partitionId.Value.ToString())) throw new FailingPartitionAlreadyExists(streamProcessorId, partitionId);
                state.FailingPartitions.Add(partitionId.Value.ToString(), new FailingPartitionState(position, retryTime, reason));
                var replaceResult = await states.ReplaceOneAsync(
                    _streamProcessorFilter.Eq(_ => _.Id, id),
                    state,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (replaceResult.MatchedCount == 0) throw new FailingPartitionDoesNotExist(streamProcessorId, partitionId);

                return state.ToRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Processing.Streams.StreamProcessorState> RemoveFailingPartition(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var states = _connection.StreamProcessorStates;
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (state == default) throw new StreamProcessorNotFound(streamProcessorId);

                if (!state.FailingPartitions.ContainsKey(partitionId.Value.ToString())) throw new FailingPartitionDoesNotExist(streamProcessorId, partitionId);

                state.FailingPartitions.Remove(partitionId.Value.ToString());
                var replaceResult = await states.ReplaceOneAsync(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)),
                    state,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (replaceResult.MatchedCount == 0) throw new FailingPartitionDoesNotExist(streamProcessorId, partitionId);

                return state.ToRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Processing.Streams.StreamProcessorState> SetFailingPartitionState(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, PartitionId partitionId, Runtime.Events.Processing.Streams.FailingPartitionState failingPartitionState, CancellationToken cancellationToken = default)
        {
            try
            {
                var states = _connection.StreamProcessorStates;
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (state == default) throw new StreamProcessorNotFound(streamProcessorId);

                if (!state.FailingPartitions.ContainsKey(partitionId.Value.ToString())) throw new FailingPartitionDoesNotExist(streamProcessorId, partitionId);

                state.FailingPartitions[partitionId.Value.ToString()] = new FailingPartitionState(failingPartitionState.Position, failingPartitionState.RetryTime, failingPartitionState.Reason);

                var replaceResult = await states.ReplaceOneAsync(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId)),
                    state,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (replaceResult.MatchedCount == 0) throw new FailingPartitionDoesNotExist(streamProcessorId, partitionId);

                return state.ToRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}