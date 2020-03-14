// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Microservice" /> attempts to subscribe to a tenant that does not exist.
    /// </summary>
    public class ProducerTenantDoesNotExist : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerTenantDoesNotExist"/> class.
        /// </summary>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        /// <param name="consumerMicroservice">The consumer <see cref="Microservice" />.</param>
        /// <param name="consumerTenant">The consumer <see cref="TenantId" />.</param>
        public ProducerTenantDoesNotExist(TenantId producerTenant, Microservice consumerMicroservice, TenantId consumerTenant)
            : base($"Tenant '{consumerTenant}' in microservice '{consumerMicroservice}' attempts to subscribe to producer tenant '{producerTenant}', but that tenant does not exist.")
        {
        }
    }
}