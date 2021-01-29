// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents the configuration for event horizon consents.
    /// </summary>
    [Name("event-horizon-consents")]
    public class EventHorizonConsentsConfiguration :
        ReadOnlyDictionary<Guid, IEnumerable<EventHorizonConsent>>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonConsentsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="TenantId"/> with <see cref="IEnumerable{T}" /> of <see cref="EventHorizonConsent"/>.</param>
        public EventHorizonConsentsConfiguration(IDictionary<Guid, IEnumerable<EventHorizonConsent>> configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}" /> list of <see cref="EventHorizonConsent" /> configured for a producer <see cref="TenantId" />.
        /// </summary>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        /// <returns>The <see cref="IEnumerable{T}" /> list of <see cref="EventHorizonConsent" /> for a producer tenant.</returns>
        public IEnumerable<EventHorizonConsent> GetConsentConfigurationsFor(TenantId producerTenant)
            => TryGetValue(producerTenant, out var consents) switch
            {
                true => consents,
                false => Enumerable.Empty<EventHorizonConsent>(),
            };
    }
}
