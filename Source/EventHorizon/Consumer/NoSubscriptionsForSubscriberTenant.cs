// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when there is no event horizon subscriptions for the given tenant.
    /// </summary>
    public class NoSubscriptionsForSubscriberTenant : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoSubscriptionsForSubscriberTenant"/> class.
        /// </summary>
        /// <param name="tenant">The subscriber <see cref="TenantId" />.</param>
        public NoSubscriptionsForSubscriberTenant(TenantId tenant)
            : base($"Tenant '{tenant}' has no event horizon subscriptions")
        {
        }
    }
}