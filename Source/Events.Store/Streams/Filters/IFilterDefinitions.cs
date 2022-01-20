// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams.Filters;

/// <summary>
/// Defines a repository for <see cref="IFilterDefinition" >filter definitions</see>.
/// </summary>
public interface IFilterDefinitions
{
    /// <summary>
    /// Persists a new <see cref="IFilterDefinition" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>
    /// A <see cref="Task" /> that, when resolved, returns a value indicating whether or not the <see cref="IFilterDefinition" />
    /// was persisted for an existing <see cref="IStreamDefinition" />.
    /// </returns>
    Task<bool> TryPersist(ScopeId scopeId, IFilterDefinition filterDefinition, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the persisted <see cref="IFilterDefinition" /> from a <see cref="StreamId" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="streamId">The <see cref="StreamId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="IFilterDefinition" /> result.</returns>
    Task<Try<IFilterDefinition>> TryGetFromStream(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken);
}