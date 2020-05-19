// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using runtime = Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon
{
    /// <summary>
    /// Extension methods for <see cref="SubscriptionState" />.
    /// </summary>
    public static class SubscriptionStateExtensions
    {
        /// <summary>
        /// Converts the <see cref="SubscriptionState" /> to the runtime representation of <see cref="runtime.StreamProcessorState" />
        /// as the SubscriptionStates are represented by an unpartitioned Stream Processor State in the runtime.
        /// </summary>
        /// <param name="state">The <see cref="StreamProcessorState" />.</param>
        /// <returns>The converted <see cref="runtime.Partitioned.StreamProcessorState" />.</returns>
        public static runtime.StreamProcessorState ToRuntimeRepresentation(this SubscriptionState state) =>
            new runtime.StreamProcessorState(
                state.Position,
                state.FailureReason,
                state.RetryTime,
                state.ProcessingAttempts,
                state.LastSuccessfullyProcessed,
                state.IsFailing);
    }
}
