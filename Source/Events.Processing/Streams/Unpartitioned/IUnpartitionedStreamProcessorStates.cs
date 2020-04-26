// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing.Streams.Unpartitioned
{
    /// <summary>
    /// Defines a system that knows about persisted <see cref="StreamProcessorState" >stream processor states</see>.
    /// </summary>
    public interface IUnpartitionedStreamProcessorStates
    {
        /// <summary>
        /// Gets a value indicating whether there is a persisted <see cref="StreamProcessorState" /> for the given <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique <see cref="StreamProcessorId" /> representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether there is a persisted <see cref="StreamProcessorState" />.</returns>
        Task<bool> HasFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the <see cref="StreamProcessorState" /> for the given <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique <see cref="StreamProcessorId" /> representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the persisted <see cref="StreamProcessorState" />.</returns>
        Task<StreamProcessorState> GetFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Persist the <see cref="StreamProcessorState" /> for <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="streamProcessorState">The <see cref="StreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Persist(StreamProcessorId streamProcessorId, StreamProcessorState streamProcessorState, CancellationToken cancellationToken);
    }
}