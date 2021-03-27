// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB
{
    /// <summary>
    /// Represents a <see cref="IProjections" />.
    /// </summary>
    [SingletonPerTenant]
    public class Projections : ProjectionsConnection, IProjections
    {
        const string ProjectionDefinitionCollectionName = "projection-definitions";

        readonly ILogger _logger;
        readonly IMongoCollection<Definition.ProjectionDefinition> _projectionDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Projections"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="DatabaseConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Projections(DatabaseConnection connection, ILogger logger)
            : base(connection)
        {
            _logger = logger;

            _projectionDefinitions = Database.GetCollection<Definition.ProjectionDefinition>(ProjectionDefinitionCollectionName);

            CreateCollectionsAndIndexesForProjectionDefinitions();
        }

        /// <inheritdoc/>
        public Task<IMongoCollection<State.Projection>> Get(ScopeId scopeId, ProjectionId projectionId, CancellationToken token)
        {
            return GetProjectionCollection(scopeId, projectionId, token);
        }

        /// <inheritdoc/>
        public Task<IMongoCollection<Definition.ProjectionDefinition>> GetDefinitions(ScopeId scopeId, CancellationToken token) =>
            scopeId == ScopeId.Default ? Task.FromResult(_projectionDefinitions) : GetScopedProjectionDefinitions(scopeId, token);

        static string CollectionNameForProjection(ProjectionId projectionId) => $"projection-{projectionId.Value}";

        static string CollectionNameForScopedStream(ScopeId scope, ProjectionId projection) => $"x-{scope.Value}-projection-{projection.Value}";

        static string CollectionNameForScopedStreamDefinitions(ScopeId scope) => $"x-{scope.Value}-{ProjectionDefinitionCollectionName}";

        Task<IMongoCollection<State.Projection>> GetProjectionCollection(ScopeId scope, ProjectionId projection, CancellationToken cancellationToken) =>
            scope == ScopeId.Default ? GetProjectionCollection(projection, cancellationToken) : GetScopedProjectionCollection(scope, projection, cancellationToken);

        async Task<IMongoCollection<State.Projection>> GetProjectionCollection(ProjectionId projection, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<State.Projection>(CollectionNameForProjection(projection));
            await CreateCollectionsAndIndexesForProjectionsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        async Task<IMongoCollection<State.Projection>> GetScopedProjectionCollection(ScopeId scope, ProjectionId projection, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<State.Projection>(CollectionNameForScopedStream(scope, projection));
            await CreateCollectionsAndIndexesForProjectionsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        async Task<IMongoCollection<Definition.ProjectionDefinition>> GetScopedProjectionDefinitions(ScopeId scope, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<Definition.ProjectionDefinition>(CollectionNameForScopedStreamDefinitions(scope));
            await CreateCollectionsIndexesForProjectionDefinitionsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }


        void CreateCollectionsAndIndexesForProjectionDefinitions()
        {
            _logger.CreatingIndexesFor(ProjectionDefinitionCollectionName);
            _projectionDefinitions.Indexes.CreateOne(new CreateIndexModel<Definition.ProjectionDefinition>(
                Builders<Definition.ProjectionDefinition>.IndexKeys
                    .Ascending(_ => _.Projection)));
        }

        async Task CreateCollectionsAndIndexesForProjectionsAsync(IMongoCollection<State.Projection> projections, CancellationToken cancellationToken)
        {
            _logger.CreatingIndexesFor(projections.CollectionNamespace.CollectionName);
            await projections.Indexes.CreateOneAsync(
                new CreateIndexModel<State.Projection>(
                    Builders<State.Projection>.IndexKeys
                        .Ascending(_ => _.Key)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task CreateCollectionsIndexesForProjectionDefinitionsAsync(IMongoCollection<Definition.ProjectionDefinition> projectionDefinitions, CancellationToken cancellationToken)
        {
            _logger.CreatingIndexesFor(projectionDefinitions.CollectionNamespace.CollectionName);
            await projectionDefinitions.Indexes.CreateOneAsync(
                new CreateIndexModel<Definition.ProjectionDefinition>(
                    Builders<Definition.ProjectionDefinition>.IndexKeys
                        .Ascending(_ => _.Projection)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
