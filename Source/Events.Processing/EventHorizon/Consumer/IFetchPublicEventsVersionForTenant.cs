// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Defines a system that can fetch the public events version for a tenant.
    /// </summary>
    public interface IFetchPublicEventsVersionForTenant
    {
        /// <summary>
        /// Writes a received event.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <returns>The task.</returns>
        Task<StreamPosition> Fetch(Microservice microservice, TenantId tenant);
    }
}