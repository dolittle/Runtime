// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines the connection with an event horizon.
    /// </summary>
    public interface IEventHorizonConnection
    {
        /// <summary>
        /// Initiates and keeps the event horizon connection going forever until cancelled.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns a <see cref="SubscriptionResponse" />.</returns>
        Task<SubscriptionResponse> InitiateAndKeepConnection();
    }
}
