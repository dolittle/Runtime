// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorStates" />.
    /// </summary>
    public class StreamProcessorStates : IStreamProcessorStates
    {
        readonly FilterDefinitionBuilder<StreamProcessorState> _streamProcessorFilter = Builders<StreamProcessorState>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStates"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public StreamProcessorStates(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> HasFor(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, CancellationToken cancellationToken)
        {
            try
            {
                var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(
                        streamProcessorId.ScopeId,
                        streamProcessorId.EventProcessorId,
                        streamProcessorId.SourceStreamId)))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                return state != default;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IStreamProcessorState> GetFor(Runtime.Events.Processing.Streams.StreamProcessorId streamProcessorId, CancellationToken cancellationToken)
        {
            try
            {
                var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.Id, new StreamProcessorId(
                        streamProcessorId.ScopeId,
                        streamProcessorId.EventProcessorId,
                        streamProcessorId.SourceStreamId)))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (state == default)
                {
                    return null;
                }

                return state.ToRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}
