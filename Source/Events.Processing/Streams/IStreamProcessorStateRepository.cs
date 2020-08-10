// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Async;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system repository for <see cref="IStreamProcessorState" >stream processor states</see>.
    /// </summary>
    public interface IStreamProcessorStateRepository
    {
        /// <summary>
        /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique <see cref="IStreamProcessorId" /> representing the <see cref="AbstractScopedStreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
        Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Persist the <see cref="IStreamProcessorState" /> for <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="streamProcessorState">The <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken);
    }
}
