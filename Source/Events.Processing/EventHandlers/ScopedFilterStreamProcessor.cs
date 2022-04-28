// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

public class ScopedFilterStreamProcessor
{
    readonly ChannelReader<EventLogBatch> _eventLogStream;
    readonly StreamId _streamId;
    readonly IWriteEventsToStreams _eventsWriter;
    readonly IResilientStreamProcessorStateRepository _stateProcessorStates;
    readonly TenantId _tenantId;
    readonly bool _partitioned;
    readonly ILogger _logger;

    StreamProcessorState _currentState;
    bool _started;

    public ScopedFilterStreamProcessor(
        ChannelReader<EventLogBatch> eventLogStream,
        StreamProcessorId streamProcessorId,
        IWriteEventsToStreams eventsWriter,
        IResilientStreamProcessorStateRepository stateProcessorStates,
        TenantId tenantId,
        bool partitioned,
        StreamProcessorState currentState,
        ILogger logger)
    {
        _eventLogStream = eventLogStream;
        _streamId = streamProcessorId.EventProcessorId.Value;
        _eventsWriter = eventsWriter;
        _stateProcessorStates = stateProcessorStates;
        _tenantId = tenantId;
        _partitioned = partitioned;
        _logger = logger;
        _currentState = currentState;
        Identifier = streamProcessorId;
    }

    /// <summary>
    /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="AbstractScopedStreamProcessor"/>.
    /// </summary>
    public IStreamProcessorId Identifier { get; }

    /// <summary>
    /// Starts the filter stream processing.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Start(CancellationToken cancellationToken)
    {
        if (_started)
        {
            throw new StreamProcessorAlreadyProcessingStream(Identifier);
        }
        _started = true;
        return BeginProcessing(cancellationToken);
    }


    async Task BeginProcessing(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var batch in _eventLogStream.ReadAllAsync(cancellationToken))
            {
                if (batch.From.Value != _currentState.Position.Value)
                {
                    throw new Exception($"Expected to get event from event log at sequence number {batch.From} but expected {_currentState.Position}");
                }
                var eventsAndPartitions = ConvertToEventsAndPartitions(batch.MatchedEvents);

                await _eventsWriter.Write(eventsAndPartitions, Identifier.ScopeId, _streamId, cancellationToken).ConfigureAwait(false);
                var newState = new StreamProcessorState(batch.To + 1, DateTimeOffset.UtcNow);
                await _stateProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
                _currentState = newState;
            }
        }
        catch (Exception ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _logger.FilterStreamProcessingForTenantFailed(ex, Identifier, _tenantId);
            }
        }
    }

    IEnumerable<(CommittedEvent, PartitionId)> ConvertToEventsAndPartitions(IEnumerable<Events.Contracts.CommittedEvent> events)
        => events
            .Select(_ => _.ToCommittedEvent())
            .Select(@event => (@event, _partitioned ? new PartitionId(@event.EventSource) : PartitionId.None));
}
