// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an execution context.
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
        /// </summary>
        /// <param name="correlation">The correlation.</param>
        /// <param name="microservice">The microservice.</param>
        /// <param name="tenant">The tenant.</param>
        public ExecutionContext(Guid correlation, Guid microservice, Guid tenant)
        {
            Correlation = correlation;
            Microservice = microservice;
            Tenant = tenant;
        }

        /// <summary>
        /// Gets or sets the <see cref="CorrelationId"/>.
        /// </summary>
        public Guid Correlation { get; set; }

        /// <summary>
        /// Gets or sets the producer <see cref="Microservice"/>.
        /// </summary>
        public Guid Microservice { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId"/> .
        /// </summary>
        public Guid Tenant { get; set; }
    }
}