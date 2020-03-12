// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Applications;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonConsents" />.
    /// </summary>
    [Singleton]
    public class EventHorizonConsents : IEventHorizonConsents
    {
        readonly EventHorizonConsentsConfiguration _consentsConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonConsents"/> class.
        /// </summary>
        /// <param name="consentsConfiguration">The <see cref="EventHorizonConsentsConfiguration" />.</param>
        public EventHorizonConsents(EventHorizonConsentsConfiguration consentsConfiguration)
        {
            _consentsConfiguration = consentsConfiguration;
        }

        /// <inheritdoc/>
        public EventHorizonConsent GetConsentFor(TenantId publisherTenant, Microservice subscriberMicroservice, TenantId subscriberTenant)
        {
            var consents = GetConsentConfigurationsFor(publisherTenant);
            if (!consents.Any()) throw new TenantHasNoEventHorizonConsents(publisherTenant);
            var matchingConsents = consents.Where(_ => _.Microservice == subscriberMicroservice && _.Tenant == subscriberTenant);
            if (matchingConsents.Count() > 1) throw new TenantHasMultipleConsentsForTenantInMicroservice(subscriberTenant, subscriberMicroservice, subscriberTenant);
            if (!matchingConsents.Any()) throw new TenantHasNoConsentForTenantInMicroservice(subscriberTenant, subscriberMicroservice, subscriberTenant);

            return matchingConsents.First().Consent;
        }

        /// <inheritdoc/>
        public IEnumerable<EventHorizonConsentConfiguration> GetConsentConfigurationsFor(TenantId publisherTenant)
        {
            if (!_consentsConfiguration.TryGetValue(publisherTenant, out var consents)) return Enumerable.Empty<EventHorizonConsentConfiguration>();
            return consents;
        }
    }
}