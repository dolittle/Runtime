// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when a Stream Processor is initialized multiple times.
    /// </summary>
    public class StreamProcessorAlreadyInitialized : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorAlreadyInitialized"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        public StreamProcessorAlreadyInitialized(IStreamProcessorId streamProcessorId)
            : base($"Stream Processor: '{streamProcessorId}' is already initialized")
        {
        }
    }
}
