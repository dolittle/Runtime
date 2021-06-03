// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Defines a factory that can create <see cref="StreamProcessor" />.
    /// </summary>
    public interface IStreamProcessorFactory
    {
        /// <summary>
        /// Creates a <see cref="StreamProcessor" />. 
        /// </summary>
        /// <param name="consent">The <see cref="ConsentId" />.</param>
        /// <param name="subscription">The <see cref="SubscriptionId" />.</param>
        /// <param name="eventsFromEventHorizonFetcher">The <see cref="EventsFromEventHorizonFetcher" />.</param>
        /// <returns></returns>
        IStreamProcessor Create(
            ConsentId consent,
            SubscriptionId subscription,
            EventsFromEventHorizonFetcher eventsFromEventHorizonFetcher);
    }
}
