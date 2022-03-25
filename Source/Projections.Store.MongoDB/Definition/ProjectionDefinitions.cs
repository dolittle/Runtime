// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store;

using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents an implementation of <see cref="IProjectionDefinitions" />.
/// </summary>
[Singleton, PerTenant]
public class ProjectionDefinitions : IProjectionDefinitions
{
    readonly IProjections _projections;
    readonly IConvertProjectionDefinition _definitionConverter;

    public ProjectionDefinitions(IProjections projections, IConvertProjectionDefinition definitionConverter)
    {
        _projections = projections;
        _definitionConverter = definitionConverter;
    }
    public async Task<Try<Store.Definition.ProjectionDefinition>> TryGet(ProjectionId projection, ScopeId scope, CancellationToken token)
    {
        try
        {
            return await OnDefinitions<Try<Store.Definition.ProjectionDefinition>>(
                scope,
                async collection =>
                {
                    var findResult = await collection
                        .Find(CreateIdFilter(projection))
                        .Project(_ => Tuple.Create(_.InitialStateRaw, _.EventSelectors, _.Copies))
                        .SingleOrDefaultAsync(token).ConfigureAwait(false);
                    if (findResult == null)
                    {
                        return Try<Store.Definition.ProjectionDefinition>.Failed(new ProjectionDefinitionDoesNotExist(projection, scope));
                    }
                    var (initialState, eventSelectors, copies) = findResult;
                    return _definitionConverter.ToRuntime(projection, scope, eventSelectors, initialState, copies);
                },
                token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    public async Task<bool> TryPersist(Store.Definition.ProjectionDefinition definition, CancellationToken token)
    {
        try
        {
            return await OnDefinitions(
                definition.Scope,
                async collection =>
                {
                    var updateResult = await collection
                        .ReplaceOneAsync(
                            CreateIdFilter(definition.Projection),
                            _definitionConverter.ToStored(definition),
                            new ReplaceOptions { IsUpsert = true },
                            token).ConfigureAwait(false);
                    return updateResult.IsAcknowledged;
                },
                token).ConfigureAwait(false);
        }
        catch (MongoWaitQueueFullException)
        {
            return false;
        }
    }

    async Task<TResult> OnDefinitions<TResult>(ScopeId scope, Func<IMongoCollection<ProjectionDefinition>, Task<TResult>> callback, CancellationToken token)
        => await callback(await _projections.GetDefinitions(scope, token).ConfigureAwait(false)).ConfigureAwait(false);

    FilterDefinition<ProjectionDefinition> CreateIdFilter(ProjectionId projection)
        => Builders<ProjectionDefinition>.Filter.Eq(_ => _.Projection, projection.Value);
}
