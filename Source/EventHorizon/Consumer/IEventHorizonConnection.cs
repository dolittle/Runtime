// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines the resilient connection with an event horizon.
    /// </summary>
    public interface IEventHorizonConnection : IDisposable
    {
        /// <summary>
        /// Initiates and keeps the event horizon connection going forever until disposed.
        /// </summary>
        void StartResilientConnection();

        /// <summary>
        /// Gets the first subscription response from the producer Runtime.
        /// </summary>
        Task<SubscriptionResponse> FirstSubscriptionResponse { get; }
    }
}
