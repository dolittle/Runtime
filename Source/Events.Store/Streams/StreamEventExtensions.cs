// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Extension methods for StreamEvent.
/// </summary>
public static class StreamEventExtensions
{
    /// <summary>
    /// Converts a <see cref="StreamEvent"/> to the protobuf <see cref="Events.Processing.Contracts.StreamEvent"/> message.
    /// </summary>
    /// <param name="streamEvent">The <see cref="StreamEvent"/> to convert.</param>
    /// <param name="scope">The <see cref="ScopeId"/> of the stream the <see cref="StreamEvent"/> belongs to.</param>
    /// <returns></returns>
    public static Events.Processing.Contracts.StreamEvent ToProtobuf(this StreamEvent streamEvent, ScopeId scope)
        => new()
        {
            Event = streamEvent.Event.ToProtobuf(),
            Partitioned = streamEvent.Partitioned,
            PartitionId = streamEvent.Partition,
            ScopeId = scope.ToProtobuf(),
        };
}
