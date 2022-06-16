// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Represents a <see cref="ExecutionContext"/>.
/// </summary>
public record ExecutionContext(
    MicroserviceId Microservice,
    TenantId Tenant,
    Version Version,
    Environment Environment,
    CorrelationId CorrelationId,
    SpanId SpanId,
    Claims Claims,
    CultureInfo CultureInfo)
{
    /// <inheritdoc/>
    public override string ToString() => $"Microservice: {Microservice.Value} Tenant: {Tenant.Value} Version: {Version} Environment: {Environment.Value} Correlation: {CorrelationId.Value} Span: {SpanId.Value} Claims: {Claims} CultureInfo: {CultureInfo}";

}
