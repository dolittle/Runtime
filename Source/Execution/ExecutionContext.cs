// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Represents a <see cref="ExecutionContext"/>.
    /// </summary>
    public class ExecutionContext : Value<ExecutionContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
        /// </summary>
        /// <param name="microservice"><see cref="Microservice"/> that is currently executing.</param>
        /// <param name="tenant"><see cref="TenantId"/> that is currently part of the <see cref="ExecutionContext"/>.</param>
        /// <param name="version"><see cref="Version" /> of the <see cref="Microservice" />.</param>
        /// <param name="environment"><see cref="Environment"/> for this <see cref="ExecutionContext"/>.</param>
        /// <param name="correlationId"><see cref="CorrelationId"/> for this <see cref="ExecutionContext"/>.</param>
        /// <param name="claims"><see cref="Claims"/> to populate with.</param>
        /// <param name="cultureInfo"><see cref="CultureInfo"/> for the <see cref="ExecutionContext"/>.</param>
        public ExecutionContext(
            Microservice microservice,
            TenantId tenant,
            Version version,
            Environment environment,
            CorrelationId correlationId,
            Claims claims,
            CultureInfo cultureInfo)
        {
            Microservice = microservice;
            Tenant = tenant;
            Version = version;
            Environment = environment;
            CorrelationId = correlationId;
            Claims = claims;
            Culture = cultureInfo;
        }

        /// <summary>
        /// Gets the <see cref="Microservice"/> for the <see cref="ExecutionContext">execution context</see>.
        /// </summary>
        public Microservice Microservice { get; }

        /// <summary>
        /// Gets the <see cref="TenantId"/> for the <see cref="ExecutionContext">execution context</see>.
        /// </summary>
        public TenantId Tenant { get; }

        /// <summary>
        /// Gets the <see cref="Version" /> of the <see cref="Microservice" />.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets the <see cref="Environment"/> for the <see cref="ExecutionContext">execution context</see>.
        /// </summary>
        public Environment Environment { get; }

        /// <summary>
        /// Gets the <see cref="CorrelationId"/> for the <see cref="ExecutionContext">execution context</see>.
        /// </summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="Claims"/> for the <see cref="ExecutionContext">execution context</see>.
        /// </summary>
        public Claims Claims { get; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> for the <see cref="ExecutionContext">execution context</see>.
        /// </summary>
        public CultureInfo Culture { get; }
    }
}
