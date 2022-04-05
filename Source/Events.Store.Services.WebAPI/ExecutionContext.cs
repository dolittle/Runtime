// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents an execution context.
/// </summary>
public record ExecutionContext(
    Guid Microservice,
    Guid Tenant,
    Version Version,
    string Environment,
    Guid CorrelationId,
    Claims Claims)
{
    /// <summary>
    /// Converts an <see cref="ExecutionContext"/> to a <see cref="Dolittle.Execution.Contracts.ExecutionContext"/>.
    /// </summary>
    /// <param name="context">The execution context to convert.</param>
    /// <returns>The converted execution context.</returns>
    public static implicit operator Dolittle.Execution.Contracts.ExecutionContext(ExecutionContext context)
    {
        var converted = new Dolittle.Execution.Contracts.ExecutionContext()
        {
            MicroserviceId = context.Microservice.ToProtobuf(),
            TenantId = context.Tenant.ToProtobuf(),
            Version = context.Version.ToProtobuf(),
            Environment = context.Environment,
            CorrelationId = context.CorrelationId.ToProtobuf(),
        };
        converted.Claims.AddRange(context.Claims.ToProtobuf());
        return converted;
    }
    
    /// <summary>
    /// Converts an <see cref="Dolittle.Execution.Contracts.ExecutionContext"/> to a <see cref="ExecutionContext"/>.
    /// </summary>
    /// <param name="context">The execution context to convert.</param>
    /// <returns>The converted execution context.</returns>
    public static implicit operator ExecutionContext(Dolittle.Execution.Contracts.ExecutionContext context)
        => new(
            context.MicroserviceId.ToGuid(),
            context.TenantId.ToGuid(),
            context.Version.ToVersion(),
            context.Environment,
            context.CorrelationId.ToGuid(),
            context.Claims.ToClaims());
}
