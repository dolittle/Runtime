// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the configuration for event horizon consents.
    /// </summary>
    [Name(ConfigurationName)]
    public class EventHorizonConsentsConfiguration :
        ReadOnlyDictionary<TenantId, IEnumerable<EventHorizonConsentConfiguration>>,
        IConfigurationObject
    {
        /// <summary>
        /// The name of the configuration.
        /// </summary>
        public const string ConfigurationName = "event-horizon-consents";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonConsentsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="TenantId"/> with <see cref="IEnumerable{T}" /> of <see cref="EventHorizonConsentConfiguration"/>.</param>
        public EventHorizonConsentsConfiguration(IDictionary<TenantId, IEnumerable<EventHorizonConsentConfiguration>> configuration)
            : base(configuration)
        {
        }
    }
}