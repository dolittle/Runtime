// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Microservice" /> attempts to subscribe to a tenant that does not exist.
    /// </summary>
    public class ProducerTenantDoesNotExist : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerTenantDoesNotExist"/> class.
        /// </summary>
        /// <param name="producer">The tenant that procues the public events.</param>
        /// <param name="microservice">The microservice that wants to subscribe to the tenant's public events.</param>
        public ProducerTenantDoesNotExist(TenantId producer, Microservice microservice)
            : base($"Microservice '{microservice}' attempts to subscribe to producer tenant '{producer}', but that tenant does not exist")
        {
        }
    }
}