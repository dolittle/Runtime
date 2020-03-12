// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the subscription to an event horizon.
    /// </summary>
    public class EventHorizonSubscription
    {
        /// <summary>
        /// Gets or sets the <see cref="Microservice" /> to receive events from.
        /// </summary>
        public Microservice Microservice { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId" /> tenant to receive events from.
        /// </summary>
        public TenantId Tenant { get; set; }

        /// <summary>
        /// Gets or sets the key for the subscription.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the target host for the subscription.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the target port for the subscription.
        /// </summary>
        public int Port { get; set; }
    }
}