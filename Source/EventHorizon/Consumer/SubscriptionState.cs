// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the current state of a <see cref="ISubscription"/>.
    /// </summary>
    public enum SubscriptionState
    {
        /// <summary>
        /// This state means that the subscription is newly created, and has not been started yet.
        /// </summary>
        Created,

        /// <summary>
        /// This state means that the subscription is currently connecting to the producer Runtime.
        /// </summary>
        Connecting,

        /// <summary>
        /// This state means that the subscription is currently connected to the producer Runtime and processing events.
        /// </summary>
        Connected
    }
}
