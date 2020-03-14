// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when an event from an event horizon is not valid.
    /// </summary>
    public class InvalidEventFromEventHorizon : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventFromEventHorizon"/> class.
        /// </summary>
        /// <param name="eventTypeId">The <see cref="ArtifactId" /> of the event type.</param>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        /// <param name="reason">The <see cref="InvalidEventFromEventHorizonReason" />.</param>
        public InvalidEventFromEventHorizon(ArtifactId eventTypeId, Microservice producerMicroservice, TenantId producerTenant, InvalidEventFromEventHorizonReason reason)
            : base($"Event '{eventTypeId}' from tenant '{producerTenant}' microservice '{producerMicroservice}' is invvalid. {reason}")
        {
        }
    }
}