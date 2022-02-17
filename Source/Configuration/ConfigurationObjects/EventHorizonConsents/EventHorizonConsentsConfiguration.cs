// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.EventHorizonConsents;

/// <summary>
/// Represents the configuration for event horizon consents.
/// </summary>
[Name("event-horizon-consents")]
public class EventHorizonConsentsConfiguration : TenantSpecificConfigurationObject<IEnumerable<EventHorizonConsentConfiguration>>
{
    /// <summary>
    /// Gets the <see cref="IEnumerable{T}" /> list of <see cref="EventHorizonConsent" /> configured for a producer <see cref="TenantId" />.
    /// </summary>
    /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
    /// <returns>The <see cref="IEnumerable{T}" /> list of <see cref="EventHorizonConsent" /> for a producer tenant.</returns>
    public IEnumerable<EventHorizonConsentConfiguration> GetConsentConfigurationsFor(Guid producerTenant)
        => TryGetValue(producerTenant, out var consents) switch
        {
            true => consents.Select(_ => _),
            false => Enumerable.Empty<EventHorizonConsentConfiguration>(),
        };
}
