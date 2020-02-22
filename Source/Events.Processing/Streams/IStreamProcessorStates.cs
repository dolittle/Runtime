// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

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
        /// Handles a <see cref="IProcessingResult" /> by changing the <see cref="StreamProcessorState" /> of the <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="processingResult">The <see cref="IProcessingResult" />.</param>
        /// <param name="currentStreamPosition">The current <see cref="StreamPosition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The new <see cref="StreamProcessorState" />.</returns>
        Task<StreamProcessorState> HandleProcessingResult(StreamProcessorId streamProcessorId, IProcessingResult processingResult, StreamPosition currentStreamPosition, CancellationToken cancellationToken = default);
    }
}