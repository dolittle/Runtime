// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Security;

using RuntimeExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Projections.Store.Services.WebAPI
{
    public record ExecutionContext(
       Guid Microservice,
       Guid Tenant,
       Versioning.Version Version,
       string Environment,
       Guid CorrelationId,
       Claim[] Claims)
    {
        public RuntimeExecutionContext ToExecutionContext()
            => new(
                Microservice,
                Tenant,
                Version,
                Environment,
                CorrelationId,
                new Claims(Claims),
                CultureInfo.InvariantCulture);
        public static ExecutionContext From(RuntimeExecutionContext executionContext)
            => new(
                executionContext.Microservice,
                executionContext.Tenant,
                executionContext.Version,
                executionContext.Environment,
                executionContext.CorrelationId,
                executionContext.Claims.ToArray());
    }

    public record CallRequestContext(Guid HeadId, ExecutionContext ExecutionContext);

    public record GetOneRequest(CallRequestContext Context, ProjectionId Projection, ScopeId Scope, ProjectionKey Key);
    public record GetAllRequest(CallRequestContext Context, ProjectionId Projection, ScopeId Scope);
}