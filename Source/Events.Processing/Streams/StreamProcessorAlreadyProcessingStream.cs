// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when a Stream Processor is started multiple times.
    /// </summary>
    public class StreamProcessorAlreadyProcessingStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorAlreadyProcessingStream"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        public StreamProcessorAlreadyProcessingStream(IStreamProcessorId streamProcessorId)
            : base($"Stream Processor: '{streamProcessorId}' is already processing stream")
        {
        }
    }
}
