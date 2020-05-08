// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Extension methods for <see cref="SubscriptionState" />.
    /// </summary>
    public static class SubscriptionStateExtensions
    {
        /// <summary>
        /// Converts the <see cref="SubscriptionState" /> to the runtime representation of <see cref="Runtime.Events.Processing.Streams.StreamProcessorState" />
        /// as the SubscriptionStates are represented by an unpartitioned Stream Processor State in the runtime.
        /// </summary>
        /// <param name="state">The <see cref="StreamProcessorState" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState" />.</returns>
        public static Runtime.Events.Processing.Streams.StreamProcessorState ToUnpartitionedStreamProcessorState(this SubscriptionState state) =>
            new Runtime.Events.Processing.Streams.StreamProcessorState(
                state.Position,
                state.FailureReason,
                state.RetryTime,
                state.ProcessingAttempts,
                state.LastSuccessfullyProcessed);

        /// <summary>
        /// Converts the <see cref="AbstractSubscriptionState" /> to the runtime representation of
        /// <see cref="Runtime.Events.Processing.Streams.StreamProcessorState" />.
        /// </summary>
        /// <param name="state">The <see cref="AbstractSubscriptionState" />.</param>
        /// <returns>The converted <see cref="IStreamProcessorState" />.</returns>
        public static IStreamProcessorState ToRuntimeRepresentation(this AbstractSubscriptionState state) =>
            (state as SubscriptionState).ToUnpartitionedStreamProcessorState() as IStreamProcessorState;
    }
}
