// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when a running Stream Processor is being unregistered from the outside.
    /// </summary>
    public class CannotUnregisterRunningStreamProcessor : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotUnregisterRunningStreamProcessor"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        public CannotUnregisterRunningStreamProcessor(StreamProcessorId streamProcessorId)
            : base($"Stream Processor: '{streamProcessorId}' is running, cannot unregister")
        {
        }
    }
}
