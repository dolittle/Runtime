// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Represents the state of a stream.
    /// </summary>
    public enum StreamProcessorState
    {
        /// <summary>The state of the <see cref="StreamProcessor">stream processor</see> when it is running.</summary>
        Running = 0,

        /// <summary>The state of the stream when it has stopped processing.</summary>
        Stopping,

        /// <summary>The state of the stream when it is retrying the last processed element.</summary>
        Retrying,
    }
}