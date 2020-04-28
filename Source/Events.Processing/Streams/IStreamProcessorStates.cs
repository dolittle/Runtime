// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system that knows about persisted <see cref="IStreamProcessorState" >stream processor states</see>.
    /// </summary>
    public interface IStreamProcessorStates
    {
        /// <summary>
        /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique <see cref="StreamProcessorId" /> representing the <see cref="AbstractStreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the tuple with a <see cref="bool" /> and the persisted <see cref="IStreamProcessorState" /> if exists.</returns>
        Task<(bool success, IStreamProcessorState streamProcessorState)> TryGetFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Persist the <see cref="IStreamProcessorState" /> for <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="streamProcessorState">The <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Persist(StreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken);
    }
}