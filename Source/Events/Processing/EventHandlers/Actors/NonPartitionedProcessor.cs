// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public class NonPartitionedProcessor : ProcessorBase
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
            while (!cancellationToken.IsCancellationRequested)
            {
                var evt = await messages.ReadAsync(cancellationToken);

                var (streamProcessorState, processingResult) = await ProcessEvent(evt, state, GetExecutionContextForEvent(evt), cancellationToken);
                while (processingResult.Retry)
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

                    (state, processingResult) = await RetryProcessingEvent(evt, state, processingResult.FailureReason,
                            AsNonPartitioned(streamProcessorState).ProcessingAttempts + 1,
                            GetExecutionContextForEvent(evt), cancellationToken);
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

    static StreamProcessorState AsNonPartitioned(IStreamProcessorState state)
    {
        if (state is not StreamProcessorState nonPartitionedState) throw new ArgumentException("State is not a non-partitioned state");

        return nonPartitionedState;
    }
}
