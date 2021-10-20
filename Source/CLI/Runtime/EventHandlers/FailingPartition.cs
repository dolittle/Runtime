// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Represents a failing Partition of a Partitioned Event Handler.
    /// </summary>
    /// <param name="Id">The Partition identifier.</param>
    /// <param name="Position">The position of the failing Event in the Stream.</param>
    /// <param name="FailureReason">The reason why the Event Handler is failing.</param>
    /// <param name="ProcessingAttempts">The number of times the Event Handler has tried to process a failing Event.</param>
    /// <param name="RetryTime">The next time to process the failed Event.</param>
    /// <param name="LastFailed">The last time processing the failed Event was attempted.</param>
    public record FailingPartition(
        string Id,
        ulong Position,
        string FailureReason,
        uint ProcessingAttempts,
        DateTimeOffset RetryTime,
        DateTimeOffset LastFailed);
}