// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents a <see cref="SubscriptionResponse" /> where the subscription failed.
    /// </summary>
    public class FailedSubscriptionResponse : SubscriptionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedSubscriptionResponse"/> class.
        /// </summary>
        /// <param name="failure">The <see cref="Failure" />.</param>
        public FailedSubscriptionResponse(Failure failure)
            : base(failure)
        {
        }
    }
}