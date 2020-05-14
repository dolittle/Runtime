// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.Partitioned;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Extension methods for stream processor.
    /// </summary>
    public static class StreamProcessorStateExtensions
    {
        /// <summary>
        /// Converts the <see cref="StreamProcessorState" /> to the runtime representation of <see cref="Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState" />.
        /// </summary>
        /// <param name="state">The <see cref="StreamProcessorState" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState" />.</returns>
        public static Runtime.Events.Processing.Streams.StreamProcessorState ToUnpartitionedStreamProcessorState(this MongoDB.Processing.Streams.StreamProcessorState state) =>
            new Runtime.Events.Processing.Streams.StreamProcessorState(
                state.Position,
                state.FailureReason,
                state.RetryTime,
                state.ProcessingAttempts,
                state.LastSuccessfullyProcessed,
                state.IsFailing);

        /// <summary>
        /// Converts the given derived class of <see cref="AbstractStreamProcessorState"/> to its corresponding runtime representation.
        /// </summary>
        /// <param name="state">The <see cref="AbstractStreamProcessorState" />.</param>
        /// <returns>The converted <see cref="IStreamProcessorState" />.</returns>
        public static IStreamProcessorState ToRuntimeRepresentation(this AbstractStreamProcessorState state)
        {
            return state switch
            {
                PartitionedStreamProcessorState partitionedState => partitionedState.ToPartitionedStreamProcessorState(),
                StreamProcessorState streamState => streamState.ToUnpartitionedStreamProcessorState(),
                _ => throw new UnsupportedStateDerivedFromAbstractStreamProcessorState(state)
            };
        }
    }
}
