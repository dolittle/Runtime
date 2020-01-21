// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the state of the processing of a stream.
    /// </summary>
    public enum StreamProcessingState
    {
        /// <summary>The state when the stream processor is waiting for an event to process.</summary>
        Waiting = 0,

        /// <summary>The state when the stream processor is processing</summary>
        Processing,

        /// <summary>The state when the processing of events has stopped.</summary>
        Stopping,

        /// <summary>The state when the processing of events is retrying to process the last processed event.</summary>
        Retrying,
    }
}