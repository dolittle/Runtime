// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public class NonPartitionedProcessor : ProcessorBase<StreamProcessorState>
{
    public NonPartitionedProcessor(
        StreamProcessorId streamProcessorId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId,
        ILogger logger)
        : base(
            streamProcessorId, processor, streamProcessorStates, executionContext, onProcessed, onFailedToProcess, tenantId, logger)
    {
    }

    

    public async Task Process(ChannelReader<StreamEvent> messages, IStreamProcessorState state, CancellationToken cancellationToken)
    {
        try
        {
            var currentState = AsNonPartitioned(state);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var evt = await messages.ReadAsync(cancellationToken);

                    (currentState, var processingResult) = await ProcessEvent(evt, currentState, GetExecutionContextForEvent(evt), cancellationToken);
                    await PersistNewState(currentState, cancellationToken);

                    while (processingResult is { Succeeded: false, Retry: true })
                    {
                        if (state.TryGetTimespanToRetry(out var retryTimeout))
                        {
                            Logger.LogInformation("Will retry processing event {evt.Position} after {Timeout}", evt, retryTimeout);
                            await Task.Delay(retryTimeout, cancellationToken);
                        }
                        else
                        {
                            Logger.LogInformation("Will retry processing event {evt.Position} directly", evt);
                        }

                        (currentState, processingResult) = await RetryProcessingEvent(evt, currentState, processingResult.FailureReason,
                            currentState.ProcessingAttempts + 1,
                            GetExecutionContextForEvent(evt), cancellationToken);
                    }

                    if (!processingResult.Succeeded)
                    {
                        Logger.StoppedFailingEventHandler(Identifier.EventProcessorId, Identifier.ScopeId, currentState.FailureReason);
                        return;
                    }
                }
                finally
                {
                    await PersistNewState(currentState, CancellationToken.None);
                }
            }
        }
        catch (OperationCanceledException e)
        {
            Logger.CancelledRunningEventHandler(e, Identifier.EventProcessorId, Identifier.ScopeId);
        }
        catch (Exception e)
        {
            Logger.ErrorWhileRunningEventHandler(e, Identifier.EventProcessorId, Identifier.ScopeId);
        }
    }

    StreamProcessorState AsNonPartitioned(IStreamProcessorState state)
    {
        switch (state)
        {
            case StreamProcessorState nonPartitionedState:
                return nonPartitionedState;

            case Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState partitionedState:
                if (!partitionedState.FailingPartitions.Any())
                {
                    Logger.LogInformation("Converting partitioned state to non-partitioned for {StreamProcessorId}", Identifier);
                    return new StreamProcessorState(partitionedState.Position, partitionedState.LastSuccessfullyProcessed);
                }

                throw new ArgumentException("State is not convertible to non-partitioned");

            default:
                throw new ArgumentException("Invalid state type");
        }
    }
}
