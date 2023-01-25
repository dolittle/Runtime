// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents a unique key for a <see cref="StreamProcessor" />.
/// </summary>
/// <param name="ScopeId">The <see cref="ScopeId" />.</param>
/// <param name="EventProcessorId"><see cref="EventProcessorId"/>.</param>
/// <param name="SourceStreamId">The <see cref="StreamId"/>.</param>
public record StreamProcessorId(ScopeId ScopeId, EventProcessorId EventProcessorId, StreamId SourceStreamId) : IStreamProcessorId
{
    /// <inheritdoc />
    public override string ToString() => $"Scope: {ScopeId.Value} Event Processor Id: {EventProcessorId.Value} Source Stream: {SourceStreamId.Value}";

    public IStreamProcessorId FromProtobuf(StreamProcessorKey streamProcessorKey)
        => throw new System.NotImplementedException();

    StreamProcessorKey IStreamProcessorId.ToProtobuf()
        => throw new System.NotImplementedException();
}
