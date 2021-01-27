// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
     /// <summary>
    /// Represents the state of a failing partition.
    /// </summary>
    /// <param name="Position">The <see cref="StreamPosition" />.</param>
    /// <param name="RetryTime">The <see cref="DateTimeOffset" /> to retry processing.</param>
    /// <param name="Reason">The reason for failing.</param>
    /// <param name="ProcessingAttempt">The number of times the Event has been processed.</param>
    /// <param name="LastFailed">The <see cref="DateTimeOffset" /> for when this partition last failed.</param>
    public record FailingPartitionState(StreamPosition Position, DateTimeOffset RetryTime, string Reason, uint ProcessingAttempts, DateTimeOffset LastFailed);
}
