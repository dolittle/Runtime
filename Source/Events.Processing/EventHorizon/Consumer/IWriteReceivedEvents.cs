// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Defines a system that knows about received events.
    /// </summary>
    public interface IWriteReceivedEvents
    {
        /// <summary>
        /// Writes a received event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="producerMicroservice">The <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The <see cref="TenantId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task.</returns>
        Task Write(CommittedEvent @event, Microservice producerMicroservice, TenantId producerTenant, CancellationToken cancellationToken = default);
    }
}