// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Defines a system that can create instances of <see cref="ExecutionContext"/>.
/// </summary>
public interface ICreateExecutionContexts
{
    /// <summary>
    /// Creates an execution context for a <see cref="TenantId"/>, <see cref="CorrelationId"/> and <see cref="Claims"/>, combined with the current configuration of the Runtime.
    /// </summary>
    /// <param name="tenant">The tenant to use in the created execution context.</param>
    /// <param name="version">The version to use in the created execution context.</param>
    /// <param name="correlation">The correlation to use in the created execution context.</param>
    /// <param name="spanId">The span to use in the created execution context.</param>
    /// <param name="claims">The claims to use in the created execution context.</param>
    /// <returns>A new <see cref="ExecutionContext"/> with the specified fields.</returns>
    ExecutionContext CreateFor(TenantId tenant, Version version, CorrelationId correlation, SpanId spanId, Claims claims);

    /// <summary>
    /// Tries to create a new <see cref="ExecutionContext"/> from the requested <see cref="ExecutionContext"/>.
    /// It will fail if the requested execution context is incompatible with the current configuration of the Runtime.
    /// </summary>
    /// <param name="requested">The requested execution context - typically from a Client.</param>
    /// <returns>A <see cref="Try{TResult}"/> with a new <see cref="ExecutionContext"/> with the requested fields, if successful.</returns>
    Try<ExecutionContext> TryCreateUsing(ExecutionContext requested);
}
