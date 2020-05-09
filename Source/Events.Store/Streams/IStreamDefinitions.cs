// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that knows about <see cref="IStreamDefinition" /> for <see cref="ITenants" />.
    /// </summary>
    public interface IStreamDefinitions
    {
        /// <summary>
        /// Try to get the shared <see cref="IStreamDefinition" /> for <see cref="ITenants.All" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> with <see cref="IStreamDefinition" /> result.</returns>
        Task<Try<IStreamDefinition>> TryGet(ScopeId scope, StreamId streamId, CancellationToken cancellationToken);

        /// <summary>
        /// Persists an <see cref="IStreamDefinition" /> for <see cref="ITenants.All" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken);
    }
}
