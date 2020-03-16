// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Exception that is thrown when a given tenant does not have any event horizon consents configured.
    /// </summary>
    public class TenantHasNoEventHorizonConsents : InvalidEventHorizonConsentsConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantHasNoEventHorizonConsents"/> class.
        /// </summary>
        /// <param name="publisherTenant">The publisher <see cref="TenantId" />.</param>
        public TenantHasNoEventHorizonConsents(TenantId publisherTenant)
            : base($"The tenant '{publisherTenant}' does not have any event horizon consents configured")
        {
        }
    }
}