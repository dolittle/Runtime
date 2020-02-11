// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Extension methods for stream processor.
    /// </summary>
    public static class StreamProcessorStateExtensions
    {
        /// <summary>
        /// Converts <see cref="StreamProcessorId" /> to the runtime representation of <see cref="Events.Processing.StreamProcessorId" />.
        /// </summary>
        /// <param name="id">The <see cref="StreamProcessorId" />.</param>
        /// <returns>The converted <see cref="Events.Processing.StreamProcessorId" />.</returns>
        public static Events.Processing.StreamProcessorId ToRuntimeRepresentation(this StreamProcessorId id) => new Events.Processing.StreamProcessorId(id.EventProcessorId, id.SourceStreamId);

        /// <summary>
        /// Converts the <see cref="FailingPartitionState" /> to the runtime representation of <see cref="Events.Processing.FailingPartitionState" />.
        /// </summary>
        /// <param name="state">The <see cref="FailingPartitionState" />.</param>
        /// <returns>The converted <see cref="Events.Processing.FailingPartitionState" />.</returns>
        public static Events.Processing.FailingPartitionState ToRuntimeRepresentation(this FailingPartitionState state) => new Events.Processing.FailingPartitionState { Position = state.Position, RetryTime = state.RetryTime };

        /// <summary>
        /// Converts the <see cref="StreamProcessorState" /> to the runtime representation of <see cref="Events.Processing.StreamProcessorState" />.
        /// </summary>
        /// <param name="state">The <see cref="StreamProcessorState" />.</param>
        /// <returns>The converted <see cref="Events.Processing.StreamProcessorState" />.</returns>
        public static Events.Processing.StreamProcessorState ToRuntimeRepresentation(this StreamProcessorState state) =>
            new Events.Processing.StreamProcessorState(state.Position, state.FailingPartitions.ToDictionary(_ => new Events.Processing.PartitionId { Value = _.Key }, _ => _.Value.ToRuntimeRepresentation()));
    }
}