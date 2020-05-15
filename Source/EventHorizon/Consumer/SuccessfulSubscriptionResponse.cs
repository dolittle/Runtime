// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.EventHorizon.Producer;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents a <see cref="SubscriptionResponse" /> where the subscription was successful.
    /// </summary>
    public class SuccessfulSubscriptionResponse : SubscriptionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulSubscriptionResponse"/> class.
        /// </summary>
        /// <param name="consentId">The <see cref="EventHorizonConsentId" />.</param>
        public SuccessfulSubscriptionResponse(EventHorizonConsentId consentId)
            : base(consentId)
        {
        }
    }
}
