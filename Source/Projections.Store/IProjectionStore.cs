// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store
{
    /// <summary>
    /// Defines the repository for <see cref="ProjectionState" />.
    /// </summary>
    public interface IProjectionStore
    {
        /// <summary>
        /// Try to get a specific projection read model by projection id, scope id and projection key.
        /// </summary>
        /// <param name="projection">The projection id.</param>
        /// <param name="scope">The scope id.</param>
        /// <param name="key">The projection key.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="ProjectionCurrentState" />.</returns>
        Task<Try<ProjectionCurrentState>> TryGet(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token);

        /// <summary>
        /// Try to get a all projection read models by projection id and scope id.
        /// </summary>
        /// <param name="projection">The projection id.</param>
        /// <param name="scope">The scope id.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="ProjectionCurrentState" />.</returns>
        Task<Try<IEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, CancellationToken token);

        /// <summary>
        /// Try to replace a specific projection read model by projection id, scope id and projection key.
        /// </summary>
        /// <param name="projection">The projection id.</param>
        /// <param name="scope">The scope id.</param>
        /// <param name="key">The projection key.</param>
        /// <param name="state">The new projection state.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully replaced.</returns>
        Task<bool> TryReplace(ProjectionId projection, ScopeId scope, ProjectionKey key, ProjectionState state, CancellationToken token);

        /// <summary>
        /// Try to remove a specific projection read model by projection id, scope id and projection key.
        /// </summary>
        /// <param name="projection">The projection id.</param>
        /// <param name="scope">The scope id.</param>
        /// <param name="key">The projection key.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully removed.</returns>
        Task<bool> TryRemove(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token);

        /// <summary>
        /// Try to drop all projection read models by projection id and scope id.
        /// </summary>
        /// <param name="projection">The projection id.</param>
        /// <param name="scope">The scope id.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the projection collection was successfully dropped.</returns>
        Task<bool> TryDrop(ProjectionId projection, ScopeId scope, CancellationToken token);
    }
}
