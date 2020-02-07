// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when a stream processor with a given <see cref="StreamProcessorId" /> was not found in the collection.
    /// </summary>
    public class StreamProcessorNotFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorNotFound"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        public StreamProcessorNotFound(StreamProcessorId streamProcessorId)
            : base($"Could not find stream processor with if '{streamProcessorId}'")
        {
        }
    }
}