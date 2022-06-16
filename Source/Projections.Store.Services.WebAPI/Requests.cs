// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using RuntimeExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Projections.Store.Services.WebAPI;

public record ExecutionContext(
    Guid Microservice,
    Guid Tenant,
    Version Version,
    string Environment,
    Guid CorrelationId,
    string SpanId,
    Claim[] Claims)
{
    public RuntimeExecutionContext ToExecutionContext()
        => new(
            Microservice,
            Tenant,
            Version,
            Environment,
            CorrelationId,
            ActivitySpanId.CreateFromString(SpanId),
            new Claims(Claims),
            CultureInfo.InvariantCulture);
    public static ExecutionContext From(RuntimeExecutionContext executionContext)
        => new(
            executionContext.Microservice,
            executionContext.Tenant,
            executionContext.Version,
            executionContext.Environment,
            executionContext.CorrelationId,
            executionContext.SpanId.Value.ToHexString(),
            executionContext.Claims.ToArray());
}

public record CallRequestContext(Guid HeadId, ExecutionContext ExecutionContext);

public record GetOneRequest(CallRequestContext Context, ProjectionId Projection, ScopeId Scope, ProjectionKey Key);
public record GetAllRequest(CallRequestContext Context, ProjectionId Projection, ScopeId Scope);
