// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when there is no consent given for an event horizon.
    /// </summary>
    public class NoConsentForEventHorizon : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoConsentForEventHorizon"/> class.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        public NoConsentForEventHorizon(EventHorizon eventHorizon)
            : base($"There is no consent given for the event horizon between consumer microservice '{eventHorizon.ConsumerMicroservice}' and tenant '{eventHorizon.ConsumerTenant}' and producer microservice '{eventHorizon.ProducerMicroservice}' and tenant '{eventHorizon.ProducerTenant}'")
        {
        }
    }
}