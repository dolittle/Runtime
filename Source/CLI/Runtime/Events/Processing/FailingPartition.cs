// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Events.Processing;

/// <summary>
/// Represents a failing Partition of a Partitioned Stream Processor.
/// </summary>
/// <param name="Id">The Partition identifier.</param>
/// <param name="Position">The position of the failing Event in the Stream.</param>
/// <param name="FailureReason">The reason why the Stream Processor is failing.</param>
/// <param name="ProcessingAttempts">The number of times the Stream Processor has tried to process a failing Event.</param>
/// <param name="RetryTime">The next time to process the failed Event.</param>
/// <param name="LastFailed">The last time processing the failed Event was attempted.</param>
public record FailingPartition(
    string Id,
    ulong Position,
    string FailureReason,
    uint ProcessingAttempts,
    DateTimeOffset RetryTime,
    DateTimeOffset LastFailed);
