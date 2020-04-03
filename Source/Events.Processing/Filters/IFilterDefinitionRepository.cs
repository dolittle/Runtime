// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a repository for <see cref="IFilterDefinition" >filter definitions</see>.
    /// </summary>
    public interface IFilterDefinitionRepository
    {
        /// <summary>
        /// Persists a <see cref="IFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The asynchronous operating of adding a persisted filter.</returns>
        Task PersistFilter(IFilterDefinition filterDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the persisted <see cref="IFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted filter definition or the given <see cref="IFilterDefinition" /> if it is not persistable or if it has not been persisted yet.</returns>
        Task<IFilterDefinition> GetPersistedFilter(IFilterDefinition filterDefinition, CancellationToken cancellationToken);
    }
}