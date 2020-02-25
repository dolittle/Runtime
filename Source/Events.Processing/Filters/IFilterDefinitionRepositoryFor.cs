// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a repository for <see cref="IFilterDefinition" /> of a specific type.
    /// </summary>
    /// <typeparam name="TDefinition">The type of <see cref="IFilterDefinition" /> it can persist.</typeparam>
    public interface IFilterDefinitionRepositoryFor<TDefinition> : ICanRemovePersistedFilterDefinition
        where TDefinition : IFilterDefinition
    {
        /// <summary>
        /// Persists a <see cref="IFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task of adding a persisted filter.</returns>
        Task PersistFilter(TDefinition filterDefinition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the persisted <see cref="IFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted filter definition or null if is not persisted.</returns>
        Task<TDefinition> GetPersistedFilter(TDefinition filterDefinition, CancellationToken cancellationToken = default);
    }
}