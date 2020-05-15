// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf;
using Dolittle.Runtime.EventHorizon.Producer;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the response of subscription request.
    /// </summary>
    public class SubscriptionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionResponse"/> class.
        /// </summary>
        /// <param name="consentId">The <see cref="EventHorizonConsentId" />.</param>
        protected SubscriptionResponse(EventHorizonConsentId consentId)
        {
            Success = true;
            ConsentId = consentId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionResponse"/> class.
        /// </summary>
        /// <param name="failure">The <see cref="Failure"/>.</param>
        protected SubscriptionResponse(Failure failure) => Failure = failure;

        /// <summary>
        /// Gets a value indicating whether the subscription is fine.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the <see cref="EventHorizonConsentId" />.
        /// </summary>
        public EventHorizonConsentId ConsentId { get; }

        /// <summary>
        /// Gets the reason for why the subscription failed.
        /// </summary>
        public Failure Failure { get; }
    }
}