// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when attempting to create a stream processor on a stream that does is not defined.
    /// </summary>
    public class CannotCreateStreamProcessorOnUndefinedStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotCreateStreamProcessorOnUndefinedStream"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        public CannotCreateStreamProcessorOnUndefinedStream(StreamProcessorId streamProcessorId)
            : base($"Stream Processor: '{streamProcessorId}' cannot be created on undefined Stream")
        {
        }
    }
}
