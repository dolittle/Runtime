// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Extension methods for <see cref="ExecutionContext" />.
/// </summary>
public static class ExecutionExtensions
{
    /// <summary>
    /// Convert <see cref="ExecutionContext" /> to <see cref="Execution.ExecutionContext" />.
    /// </summary>
    /// <param name="executionContext"><see cref="ExecutionContext" />.</param>
    /// <returns>Converted <see cref="Execution.ExecutionContext" />.</returns>
    public static Execution.ExecutionContext ToExecutionContext(this ExecutionContext executionContext) =>
        new(
            executionContext.Microservice,
            executionContext.Tenant,
            executionContext.Version.ToVersion(),
            executionContext.Environment,
            executionContext.Correlation,
            EmptyStoredSpanId(executionContext.SpanId) ? null : ActivitySpanId.CreateFromBytes(executionContext.SpanId),
            executionContext.Claims.ToClaims(),
            CultureInfo.InvariantCulture);

    /// <summary>
    /// Convert <see cref="Execution.ExecutionContext" /> to <see cref="ExecutionContext" />.
    /// </summary>
    /// <param name="executionContext"><see cref="Execution.ExecutionContext" />.</param>
    /// <returns>Converted <see cref="ExecutionContext" />.</returns>
    public static ExecutionContext ToStoreRepresentation(this Execution.ExecutionContext executionContext)
    {
        var span = new byte[8];
        executionContext.SpanId?.CopyTo(span);
        return new ExecutionContext(
            executionContext.CorrelationId,
            span,
            executionContext.Microservice,
            executionContext.Tenant,
            executionContext.Version.ToStoreRepresentation(),
            executionContext.Environment,
            executionContext.Claims.ToStoreRepresentation());
    }

    static bool EmptyStoredSpanId(byte[] spanId) => spanId == null || spanId.Length == 0 || spanId.All(_ => _ == byte.MinValue);
}
