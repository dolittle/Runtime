// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Get
{
    /// <summary>
    /// Represents a detailed view of an unpartitioned Event Handler state.
    /// </summary>
    /// <param name="Tenant">The Tenant.</param>
    /// <param name="Position">The stream position-</param>
    /// <param name="Status">The status.</param>
    /// <param name="FailureReason">The reason for failure.</param>
    /// <param name="RetryTime">The retry time.</param>
    /// <param name="ProcessingAttempts">The number of processing attempts.</param>
    /// <param name="LastSuccessfullyProcessed">When the last event was successfully processed</param>
    public record UnpartitionedEventHandlerDetailedView(
        Guid Tenant,
        ulong Position,
        string Status,
        string FailureReason,
        DateTimeOffset RetryTime,
        uint ProcessingAttempts,
        DateTimeOffset LastSuccessfullyProcessed);
}
