// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Represents the status of an Unpartitioned Event Handler for a specific Tenant.
    /// </summary>
    /// <param name="TenantId">The identifier of the Tenant.</param>
    /// <param name="Position">The position of the next Event the Event Handler will process.</param>
    /// <param name="IsFailing">Whether the Event Handler is currently failing or not.</param>
    /// <param name="FailureReason">The reason why the Event Handler is failing (if it is).</param>
    /// <param name="ProcessingAttempts">The number of times the Event Handler has tried to process a failing Event.</param>
    /// <param name="RetryTime">The next time to process the failed Event.</param>
    /// <param name="LastSuccessfullyProcessed">When the last successfully processing of an Event was.</param>
    public record UnpartitionedTenantScopedStreamProcessorStatus(
            Guid TenantId,
            ulong Position,
            bool IsFailing,
            string FailureReason,
            uint ProcessingAttempts,
            DateTimeOffset RetryTime,
            DateTimeOffset LastSuccessfullyProcessed)
        : TenantScopedStreamProcessorStatus(TenantId, Position, LastSuccessfullyProcessed);
}