// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when trying to use <see cref="SubscriptionId"/> with an unsupported <see cref="IStreamProcessorState"/>.
    /// </summary>
    public class UnsupportedStreamProcessorStatewithSubscriptionId : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedStreamProcessorStatewithSubscriptionId"/> class.
        /// </summary>
        /// <param name="id">The <see cref="SubscriptionId"/>.</param>
        /// <param name="state">The given <see cref="IStreamProcessorState"/>.</param>
        public UnsupportedStreamProcessorStatewithSubscriptionId(SubscriptionId id, IStreamProcessorState state)
            : base($"StreamProcessorState {state} can't be used with SubscriptionId {id}.")
        {
        }
    }
}
