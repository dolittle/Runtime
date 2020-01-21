// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Represents the state of the processing of a stream.
    /// </summary>
    public enum StreamProcessingState
    {
        /// <summary>The state when the processing of a stream is running as normal.</summary>
        Running = 0,

        /// <summary>The state when the processing of a stream has stopped.</summary>
        Stopping,

        /// <summary>The state when the processing of a stream is retrying to process the last processed element.</summary>
        Retrying,
    }
}