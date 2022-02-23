// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store;

using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB;

/// <summary>
/// Represents a <see cref="IProjections" />.
/// </summary>
[Singleton, PerTenant]
public class Projections : ProjectionsConnection, IProjections
{
    const string ProjectionDefinitionCollectionName = "projection-definitions";

    readonly IMongoCollection<Definition.ProjectionDefinition> _projectionDefinitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="Projections"/> class.
    /// </summary>
    /// <param name="connection">The <see cref="DatabaseConnection" />.</param>
    public Projections(IDatabaseConnection connection)
        : base(connection)
    {
        _projectionDefinitions = Database.GetCollection<Definition.ProjectionDefinition>(ProjectionDefinitionCollectionName);
    }

    /// <inheritdoc/>
    public Task<IMongoCollection<State.Projection>> GetStates(ScopeId scopeId, ProjectionId projectionId, CancellationToken token)
    {
        return GetProjectionCollection(scopeId, projectionId);
    }

    /// <inheritdoc/>
    public Task<IMongoCollection<Definition.ProjectionDefinition>> GetDefinitions(ScopeId scopeId, CancellationToken token) =>
        scopeId == ScopeId.Default ? Task.FromResult(_projectionDefinitions) : GetScopedProjectionDefinitions(scopeId);

    static string CollectionNameForProjection(ProjectionId projectionId) => $"projection-{projectionId.Value}";

    static string CollectionNameForScopedProjection(ScopeId scope, ProjectionId projection) => $"x-{scope.Value}-projection-{projection.Value}";

    static string CollectionNameForScopedStreamDefinitions(ScopeId scope) => $"x-{scope.Value}-{ProjectionDefinitionCollectionName}";

    Task<IMongoCollection<State.Projection>> GetProjectionCollection(ScopeId scope, ProjectionId projection) =>
        scope == ScopeId.Default ? GetProjectionCollection(projection) : GetScopedProjectionCollection(scope, projection);

    Task<IMongoCollection<State.Projection>> GetProjectionCollection(ProjectionId projection)
    {
        return Task.FromResult(Database.GetCollection<State.Projection>(CollectionNameForProjection(projection)));
    }

    Task<IMongoCollection<State.Projection>> GetScopedProjectionCollection(ScopeId scope, ProjectionId projection)
    {
        return Task.FromResult(Database.GetCollection<State.Projection>(CollectionNameForScopedProjection(scope, projection)));
    }

    Task<IMongoCollection<Definition.ProjectionDefinition>> GetScopedProjectionDefinitions(ScopeId scope)
    {
        return Task.FromResult(Database.GetCollection<Definition.ProjectionDefinition>(CollectionNameForScopedStreamDefinitions(scope)));
    }
}
