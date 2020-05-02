// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Async;

namespace Dolittle.Runtime.Events.Store.Streams.Filters
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
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task PersistFilter(IFilterDefinition filterDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the persisted <see cref="IFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="IFilterDefinition" /> result.</returns>
        Task<Try<IFilterDefinition>> TryGetPersistedFilter(IFilterDefinition filterDefinition, CancellationToken cancellationToken);
    }
}