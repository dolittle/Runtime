// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dolittle.Applications;
using Dolittle.Collections;
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
            Keys.ForEach(_ => ThrowIfInvalidSubscriptions(_, this[_]));
        }

        void ThrowIfInvalidSubscriptions(TenantId subscriberTenant, IEnumerable<EventHorizonSubscription> subscriptions)
        {
            ThrowIfDuplicateTenantAndMicroservicePair(subscriberTenant, subscriptions);
        }

        void ThrowIfDuplicateTenantAndMicroservicePair(TenantId subscriberTenant, IEnumerable<EventHorizonSubscription> subscriptions)
        {
            var map = new Dictionary<Microservice, IEnumerable<TenantId>>();

            subscriptions.ForEach(_ =>
            {
                if (!map.ContainsKey(_.Microservice))
                {
                    map.Add(_.Microservice, new TenantId[] { _.Tenant });
                }
                else
                {
                    var tenants = map[_.Microservice];
                    if (tenants.Contains(_.Tenant)) throw new TenantHasMultipleSubscriptionsToSameTenantInMicroservice(subscriberTenant, _.Microservice, _.Tenant);
                    else map[_.Microservice] = tenants.Append(_.Tenant);
                }
            });
        }
    }
}