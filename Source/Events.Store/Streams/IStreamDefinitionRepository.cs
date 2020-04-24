// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a repository for <see cref="StreamDefinition" />.
    /// </summary>
    public interface IStreamDefinitionRepository
    {
        /// <summary>
        /// Checks whether there is a persisted <see cref="StreamDefinition" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether there is a <see cref="StreamDefinition" /> persisted.</returns>
        Task<bool> HasFor(ScopeId scope, StreamId stream);

        /// <summary>
        /// Gets the persisted <see cref="StreamDefinition" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamDefinition" />.</returns>
        Task<StreamDefinition> GetFor(ScopeId scope, StreamId stream, CancellationToken cancellationToken);

        /// <summary>
        /// Persists a <see cref="StreamDefinition" />.
        /// </summary>
        /// <param name="streamDefinition">The <see cref="StreamDefinition" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Persist(StreamDefinition streamDefinition);
    }
}