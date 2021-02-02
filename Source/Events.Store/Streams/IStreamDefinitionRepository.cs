// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a repository for <see cref="IStreamDefinition" />.
    /// </summary>
    public interface IStreamDefinitionRepository
    {
        /// <summary>
        /// Gets the persisted <see cref="IStreamDefinition" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="IStreamDefinition" /> result.</returns>
        Task<Try<IStreamDefinition>> TryGet(ScopeId scope, StreamId stream, CancellationToken cancellationToken);

        /// <summary>
        /// Persists a <see cref="IStreamDefinition" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken);
    }
}
