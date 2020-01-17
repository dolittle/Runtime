// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// The state of an <see cref="ICanProcessStreamOfEvents" >event processor</see>.
    /// </summary>
    public enum ProcessingState
    {
        /// <summary>A null state of an <see cref="ICanProcessStreamOfEvents" >event processor</see> when it has been initialized.</summary>
        NullState = -1,

        /// <summary>The state of an <see cref="ICanProcessStreamOfEvents" >event processor</see> when it has successfully processed an <see cref="EventEnvelope">event</see>.</summary>
        Ok = 0,

        /// <summary>The state of an <see cref="ICanProcessStreamOfEvents" >event processor</see> when it has unsuccessfully processed an <see cref="EventEnvelope">event</see> and it has to stop processing.</summary>
        Stop,

        /// <summary>The state of an <see cref="ICanProcessStreamOfEvents" >event processor</see> when it has unsuccessfully processed an <see cref="EventEnvelope">event</see>, but it will retry later at some point later.</summary>
        Retry,

        /// <summary>The state of an <see cref="ICanProcessStreamOfEvents" >event processor</see> when it has skipped the processing of an <see cref="EventEnvelope">event</see>.</summary>
        Ignore
    }
}