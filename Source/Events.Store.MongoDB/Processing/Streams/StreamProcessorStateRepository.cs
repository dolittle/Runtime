// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="Store.Streams.IStreamProcessorStates" />.
/// </summary>
[PerTenant]
public class StreamProcessorStateRepository : StreamProcessorStateRepositoryBase<StreamProcessorId, IStreamProcessorState, AbstractStreamProcessorState>, IStreamProcessorStateRepository
{
    readonly FilterDefinitionBuilder<AbstractStreamProcessorState> _streamProcessorFilter = Builders<AbstractStreamProcessorState>.Filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateRepository"/> class.
    /// </summary>
    /// <param name="streamProcessorStateCollections">The <see cref="IStreamProcessorStateCollections" />.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    public StreamProcessorStateRepository(IStreamProcessorStateCollections streamProcessorStateCollections, ILogger logger)
        : base(streamProcessorStateCollections.Get, logger)
    {
    }

    /// <inheritdoc />
    public IAsyncEnumerable<StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>> GetNonScoped(CancellationToken cancellationToken)
        => GetForScope(ScopeId.Default, cancellationToken);
    
    protected override FilterDefinition<AbstractStreamProcessorState> CreateFilter(StreamProcessorId id) =>
        _streamProcessorFilter.Eq(_ => _.EventProcessor, id.EventProcessorId.Value)
        & _streamProcessorFilter.Eq(_ => _.SourceStream, id.SourceStreamId.Value);

    protected override AbstractStreamProcessorState CreateDocument(StreamProcessorId id, IStreamProcessorState state)
        => state switch
        {
            Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState partitionedStreamProcessorState => new Partitioned.PartitionedStreamProcessorState(
                id.EventProcessorId,
                id.SourceStreamId,
                partitionedStreamProcessorState.Position,
                partitionedStreamProcessorState.FailingPartitions.ToDictionary(
                    kvp => kvp.Key.Value.ToString(),
                    kvp => new FailingPartitionState(
                        kvp.Value.Position,
                        kvp.Value.RetryTime.UtcDateTime,
                        kvp.Value.Reason,
                        kvp.Value.ProcessingAttempts,
                        kvp.Value.LastFailed.UtcDateTime)),
                partitionedStreamProcessorState.LastSuccessfullyProcessed.UtcDateTime),
            Runtime.Events.Processing.Streams.StreamProcessorState streamProcessorState => new StreamProcessorState(
                id.EventProcessorId,
                id.SourceStreamId,
                streamProcessorState.Position,
                streamProcessorState.RetryTime.UtcDateTime,
                streamProcessorState.FailureReason,
                streamProcessorState.ProcessingAttempts,
                streamProcessorState.LastSuccessfullyProcessed.UtcDateTime,
                streamProcessorState.IsFailing)
        };

    protected override StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState> ConvertToStateWithId(ScopeId scope, AbstractStreamProcessorState document)
        => new(new StreamProcessorId(scope, document.EventProcessor, document.SourceStream), document.ToRuntimeRepresentation());
}
