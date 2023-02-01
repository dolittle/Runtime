// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamProcessors" />.
/// </summary>
[Singleton]
public class StreamProcessors : IStreamProcessors
{
    readonly Func<StreamProcessorId, EventProcessorKind, IStreamDefinition, Action, Func<TenantId, IEventProcessor>, ExecutionContext, CancellationToken, StreamProcessor> _createStreamProcessor;
    readonly IMetricsCollector _metrics;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly ILogger _logger;
    readonly ConcurrentDictionary<StreamProcessorId, StreamProcessor> _streamProcessors = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
    /// </summary>
    public StreamProcessors(
        Func<StreamProcessorId, EventProcessorKind, IStreamDefinition, Action, Func<TenantId, IEventProcessor>, ExecutionContext, CancellationToken, StreamProcessor> createStreamProcessor,
        IMetricsCollector metrics,
        ICreateExecutionContexts executionContextCreator,
        ILogger logger)
    {
        _createStreamProcessor = createStreamProcessor;
        _metrics = metrics;
        _executionContextCreator = executionContextCreator;
        _logger = logger;
    }

    /// <inheritdoc />
    public Try<StreamProcessor> TryCreateAndRegister(
        ScopeId scopeId,
        EventProcessorId eventProcessorId,
        EventProcessorKind eventProcessorKind,
        IStreamDefinition sourceStreamDefinition,
        Func<TenantId, IEventProcessor> getEventProcessor,
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        try
        {
            _metrics.IncrementRegistrations(eventProcessorKind);
            var createExecutionContext = _executionContextCreator.TryCreateUsing(executionContext);
            if (!createExecutionContext.Success)
            {
                // TODO: Logging
                _metrics.IncrementFailedRegistrations(eventProcessorKind);
                return createExecutionContext.Exception;
            }
            
            var streamProcessorId = new StreamProcessorId(scopeId, eventProcessorId, sourceStreamDefinition.StreamId);
            if (_streamProcessors.ContainsKey(streamProcessorId))
            {
                Log.StreamProcessorAlreadyRegistered(_logger, streamProcessorId);
                _metrics.IncrementFailedRegistrations(eventProcessorKind);
                return new StreamProcessorAlreadyRegistered(streamProcessorId);
            }
            
            var streamProcessor = _createStreamProcessor(
                streamProcessorId,
                eventProcessorKind,
                sourceStreamDefinition,
                () => Unregister(streamProcessorId),
                getEventProcessor,
                createExecutionContext.Result,
                cancellationToken);

            if (!_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
            {
                Log.StreamProcessorAlreadyRegistered(_logger, streamProcessorId);
                _metrics.IncrementFailedRegistrations(eventProcessorKind);
                return new StreamProcessorAlreadyRegistered(streamProcessorId);
            }

            Log.StreamProcessorSuccessfullyRegistered(_logger, streamProcessorId);
            return streamProcessor;
        }
        catch (Exception ex)
        {
            _metrics.IncrementFailedRegistrations(eventProcessorKind);
            return ex;
        }

    }
    /// <inheritdoc />
    public Task<Try<StreamPosition>> ReprocessEventsFrom(StreamProcessorId streamProcessorId, TenantId tenant, StreamPosition position)
        => _streamProcessors.TryGetValue(streamProcessorId, out var streamProcessor)
            ? streamProcessor.SetToPosition(tenant, position)
            : Task.FromResult<Try<StreamPosition>>(new StreamProcessorNotRegistered(streamProcessorId));

    /// <inheritdoc />
    public async Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> ReprocessAllEvents(StreamProcessorId streamProcessorId)
        => _streamProcessors.TryGetValue(streamProcessorId, out var streamProcessor)
            ? Try<IDictionary<TenantId, Try<StreamPosition>>>.Succeeded(await streamProcessor.SetToInitialPositionForAllTenants().ConfigureAwait(false))
            : new StreamProcessorNotRegistered(streamProcessorId); 

    void Unregister(StreamProcessorId id)
    {
        StreamProcessor? existing;
        do
        {
            _streamProcessors.TryRemove(id, out existing);
        }
        while (existing != default);
        Log.StreamProcessorUnregistered(_logger, id);
    }
}
