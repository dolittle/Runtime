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
        /// Gets a value indicating whether there is a persisted <see cref="IStreamProcessorState" /> for the given <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique <see cref="StreamProcessorId" /> representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether there is a persisted <see cref="IStreamProcessorState" />.</returns>
        Task<bool> HasFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="StreamProcessorId" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique <see cref="StreamProcessorId" /> representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted <see cref="IStreamProcessorState" />.</returns>
        Task<IStreamProcessorState> GetFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken);
    }
}