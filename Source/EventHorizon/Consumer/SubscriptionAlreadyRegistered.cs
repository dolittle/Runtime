// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Subscription" /> has already been registered.
    /// </summary>
    public class SubscriptionAlreadyRegistered : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="SubscriptionAlreadyRegistered" /> class.
        /// </summary>
        /// <param name="identifier">The <see cref="SubscriptionId" />.</param>
        /// <param name="consent">The <see cref="ConsentId" />.</param>
        public SubscriptionAlreadyRegistered(SubscriptionId identifier, ConsentId consent)
            : base($"Subscription {identifier} with consent {consent} has already been registered")
        {
        }
    }
}