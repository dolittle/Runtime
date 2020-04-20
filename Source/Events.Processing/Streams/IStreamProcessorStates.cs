// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system that knows how to handle <see cref="StreamProcessorState" />.
    /// </summary>
    public interface IStreamProcessorStates
    {
        /// <summary>
        /// Gets the <see cref="IFailingPartitions" />.
        /// </summary>
        IFailingPartitions FailingPartitions { get; }

        /// <summary>
        /// Gets the stored <see cref="StreamProcessorState" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The stored <see cref="StreamProcessorState" />.</returns>
        Task<StreamProcessorState> GetStoredStateFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Handles the processing an event for a <see cref="StreamProcessor" /> by changing the <see cref="StreamProcessorState" /> of the <see cref="StreamProcessor" /> according to the result of the processing.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamEvent">The <see cref="StreamEvent" />.</param>
        /// <param name="currentState">The current <see cref="StreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The new <see cref="StreamProcessorState" />.</returns>
        Task<StreamProcessorState> ProcessEventAndChangeStateFor(StreamProcessorId streamProcessorId, IEventProcessor eventProcessor, StreamEvent streamEvent, StreamProcessorState currentState, CancellationToken cancellationToken);
    }
}