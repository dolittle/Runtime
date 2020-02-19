// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents the subscriptions for a microservice.
    /// </summary>
    public class Subscriptions
        : ReadOnlyDictionary<TenantId, IEnumerable<TenantId>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public Subscriptions(IDictionary<TenantId, IEnumerable<TenantId>> dictionary)
            : base(dictionary)
        {
        }
    }
}