// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Extension methods for stream processor.
    /// </summary>
    public static class StreamProcessorStateExtensions
    {
        /// <summary>
        /// Converts <see cref="StreamProcessorId" /> to the runtime representation of <see cref="Runtime.Events.Processing.Streams.StreamProcessorId" />.
        /// </summary>
        /// <param name="id">The <see cref="StreamProcessorId" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Processing.Streams.StreamProcessorId" />.</returns>
        public static Runtime.Events.Processing.Streams.StreamProcessorId ToRuntimeRepresentation(this StreamProcessorId id) => new Runtime.Events.Processing.Streams.StreamProcessorId(id.EventProcessorId, id.SourceStreamId);

        /// <summary>
        /// Converts the <see cref="FailingPartitionState" /> to the runtime representation of <see cref="Runtime.Events.Processing.Streams.FailingPartitionState" />.
        /// </summary>
        /// <param name="state">The <see cref="FailingPartitionState" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Processing.Streams.FailingPartitionState" />.</returns>
        public static Runtime.Events.Processing.Streams.FailingPartitionState ToRuntimeRepresentation(this FailingPartitionState state) => new Runtime.Events.Processing.Streams.FailingPartitionState { Position = state.Position, RetryTime = state.RetryTime, Reason = state.Reason };

        /// <summary>
        /// Converts the <see cref="StreamProcessorState" /> to the runtime representation of <see cref="Runtime.Events.Processing.Streams.StreamProcessorState" />.
        /// </summary>
        /// <param name="state">The <see cref="StreamProcessorState" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Processing.Streams.StreamProcessorState" />.</returns>
        public static Runtime.Events.Processing.Streams.StreamProcessorState ToRuntimeRepresentation(this StreamProcessorState state) =>
            new Runtime.Events.Processing.Streams.StreamProcessorState(state.Position, state.FailingPartitions.ToDictionary(_ => new PartitionId { Value = _.Key }, _ => _.Value.ToRuntimeRepresentation()));
    }
}