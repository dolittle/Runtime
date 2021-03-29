// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB
{
    /// <summary>
    /// Defines a system that knows projections.
    /// </summary>
    public interface IProjections : IProjectionsConnection
    {
        /// <summary>
        /// Gets the projection states collection for a projection.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="State.Projection" />.</returns>
        Task<IMongoCollection<State.Projection>> GetStates(ScopeId scopeId, ProjectionId projectionId, CancellationToken token);

        /// <summary>
        /// Gets the projection definitions collection for a scope.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="Definition.ProjectionDefinition" />.</returns>
        Task<IMongoCollection<Definition.ProjectionDefinition>> GetDefinitions(ScopeId scopeId, CancellationToken token);
    }
}
