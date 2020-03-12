// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the configuration for event horizon subscriptions.
    /// </summary>
    [Name(ConfigurationName)]
    public class EventHorizonSubscriptionsConfiguration :
        ReadOnlyDictionary<TenantId, IReadOnlyList<EventHorizonSubscription>>,
        IConfigurationObject
    {
        /// <summary>
        /// The name of the <see cref="IConfigurationObject" />.
        /// </summary>
        public const string ConfigurationName = "event-horizon-subscriptions";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonSubscriptionsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="TenantId"/> with <see cref="IReadOnlyList{T}" /> of <see cref="EventHorizonSubscription"/>.</param>
        public EventHorizonSubscriptionsConfiguration(IDictionary<TenantId, IReadOnlyList<EventHorizonSubscription>> configuration)
            : base(configuration)
        {
        }
    }
}