// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorStates" />.
    /// </summary>
    [SingletonPerTenant]
    public class StreamProcessorStates : EventStoreConnection, IStreamProcessorStates
    {
        const string StreamProcessorStateCollectionName = "stream-processor-states";

        readonly ILogger _logger;
        readonly IMongoCollection<MongoDB.Processing.Streams.AbstractStreamProcessorState> _streamProcessorStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStates"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="DatabaseConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessorStates(DatabaseConnection connection, ILogger logger)
            : base(connection)
        {
            _logger = logger;

            _streamProcessorStates = Database.GetCollection<MongoDB.Processing.Streams.AbstractStreamProcessorState>(StreamProcessorStateCollectionName);
            CreateCollectionsAndIndexesForStreamProcessorStates();
        }

        /// <inheritdoc/>
        public Task<IMongoCollection<AbstractStreamProcessorState>> Get(ScopeId scopeId, CancellationToken token) =>
            scopeId == ScopeId.Default ? Task.FromResult(_streamProcessorStates) : GetScopedStreamProcessorStateCollection(scopeId, token);

        static string CollectionNameForScopedStreamProcessorStates(ScopeId scope) => $"x-{scope.Value}-{StreamProcessorStateCollectionName}";

        async Task<IMongoCollection<AbstractStreamProcessorState>> GetScopedStreamProcessorStateCollection(
            ScopeId scope,
            CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<AbstractStreamProcessorState>(CollectionNameForScopedStreamProcessorStates(scope));
            await CreateCollectionsAndIndexesForStreamProcessorStatesAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        /// <summary>
        /// Creates the compound index for <see cref="StreamProcessorState"/>.
        /// </summary>
        void CreateCollectionsAndIndexesForStreamProcessorStates()
        {
            Log.CreatingIndexesFor(_logger, StreamProcessorStateCollectionName);
            _streamProcessorStates.Indexes.CreateOne(
                new CreateIndexModel<AbstractStreamProcessorState>(
                    Builders<AbstractStreamProcessorState>.IndexKeys
                        .Ascending(_ => _.EventProcessor)
                        .Ascending(_ => _.SourceStream),
                    new CreateIndexOptions { Unique = true }));
        }

        /// <summary>
        /// Creates the compound index for <see cref="StreamProcessorState"/>.
        /// </summary>
        /// <param name="streamProcessorStates">Collection of <see cref="StreamProcessorState"/> to add indexes to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Task.</returns>
        async Task CreateCollectionsAndIndexesForStreamProcessorStatesAsync(
            IMongoCollection<AbstractStreamProcessorState> streamProcessorStates,
            CancellationToken cancellationToken)
        {
            Log.CreatingIndexesFor(_logger, streamProcessorStates.CollectionNamespace.CollectionName);
            await streamProcessorStates.Indexes.CreateOneAsync(
                new CreateIndexModel<AbstractStreamProcessorState>(
                    Builders<AbstractStreamProcessorState>.IndexKeys
                        .Ascending(_ => _.EventProcessor)
                        .Ascending(_ => _.SourceStream),
                    new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
