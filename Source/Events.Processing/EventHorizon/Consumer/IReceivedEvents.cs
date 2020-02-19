// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Defines a system that knows about received events.
    /// </summary>
    public interface IReceivedEvents
    {
        /// <summary>
        /// Writes a received event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task.</returns>
        Task Write(CommittedEvent @event, Microservice microservice, TenantId tenant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the next version of the public events for a tenant in a microservice.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>Next version of the public events for a tenant in a microservice.</returns>
        Task<StreamPosition> GetVersionForTenant(Microservice microservice, TenantId tenant, CancellationToken cancellationToken = default);
    }
}