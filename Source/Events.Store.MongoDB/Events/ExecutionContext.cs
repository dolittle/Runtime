// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Execution;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Represents an execution context.
/// </summary>
public class ExecutionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
    /// </summary>
    /// <param name="correlation">The correlation.</param>
    /// <param name="span">The 16 character hex string span id.</param>
    /// <param name="microservice">The microservice.</param>
    /// <param name="tenant">The tenant.</param>
    /// <param name="version">The version.</param>
    /// <param name="environment">The environment.</param>
    /// <param name="claims">The claims.</param>
    public ExecutionContext(Guid correlation, string span, Guid microservice, Guid tenant, Version version, string environment, IEnumerable<Claim> claims)
    {
        Correlation = correlation;
        SpanId = span;
        Microservice = microservice;
        Tenant = tenant;
        Version = version;
        Environment = environment;
        Claims = claims;
    }

    /// <summary>
    /// Gets or sets the <see cref="CorrelationId"/>.
    /// </summary>
    public Guid Correlation { get; set; }
    
    /// <summary>
    /// Gets or sets the span id 16 character hex string.
    /// </summary>
    [BsonDefaultValue(Runtime.Execution.SpanId.EmptyHexString)]
    public string SpanId { get; set; }

    /// <summary>
    /// Gets or sets the producer <see cref="Microservice"/>.
    /// </summary>
    public Guid Microservice { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="TenantId"/> .
    /// </summary>
    public Guid Tenant { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Version" />.
    /// </summary>
    public Version Version { get; set; }

    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// Gets or sets the claims.
    /// </summary>
    public IEnumerable<Claim> Claims { get; set; }
}
