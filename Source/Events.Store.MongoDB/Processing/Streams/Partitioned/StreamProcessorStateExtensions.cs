// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using runtime = Dolittle.Runtime.Events.Processing.Streams.Partitioned;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.Partitioned
{
    /// <summary>
    /// Extension methods for stream processor.
    /// </summary>
    public static class StreamProcessorStateExtensions
    {
        /// <summary>
        /// Converts the <see cref="FailingPartitionState" /> to the runtime representation of <see cref="runtime.FailingPartitionState" />.
        /// </summary>
        /// <param name="state">The <see cref="FailingPartitionState" />.</param>
        /// <returns>The converted <see cref="runtime.FailingPartitionState" />.</returns>
        public static runtime.FailingPartitionState ToRuntimeRepresentation(this FailingPartitionState state) =>
            new(state.Position, state.RetryTime, state.Reason, state.ProcessingAttempts, state.LastFailed);
    }
}
