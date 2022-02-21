// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamProcessors" />.
/// </summary>
[Singleton]
public class StreamProcessors : IStreamProcessors
{
    readonly IPerformActionsForAllTenants _forAllTenants;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;
    readonly ConcurrentDictionary<StreamProcessorId, StreamProcessor> _streamProcessors = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
    /// </summary>
    /// <param name="forAllTenants">The <see cref="IPerformActionsForAllTenants" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public StreamProcessors(
        IPerformActionsForAllTenants forAllTenants,
        ILoggerFactory loggerFactory)
    {
        _forAllTenants = forAllTenants;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StreamProcessors>();
    }

    /// <inheritdoc />
    public Try<StreamProcessor> TryCreateAndRegister(
        ScopeId scopeId,
        EventProcessorId eventProcessorId,
        IStreamDefinition sourceStreamDefinition,
        Func<IServiceProvider, IEventProcessor> getEventProcessor,
        CancellationToken cancellationToken)
    {
        try
        {
            var streamProcessorId = new StreamProcessorId(scopeId, eventProcessorId, sourceStreamDefinition.StreamId);
            if (_streamProcessors.ContainsKey(streamProcessorId))
            {
                Log.StreamProcessorAlreadyRegistered(_logger, streamProcessorId);
                return new StreamProcessorAlreadyRegistered(streamProcessorId);
            }

            var streamProcessor = new StreamProcessor(
                streamProcessorId,
                _forAllTenants,
                sourceStreamDefinition,
                getEventProcessor,
                () => Unregister(streamProcessorId),
                _loggerFactory.CreateLogger<StreamProcessor>(),
                cancellationToken);
            if (!_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
            {
                Log.StreamProcessorAlreadyRegistered(_logger, streamProcessorId);
                return new StreamProcessorAlreadyRegistered(streamProcessorId);
            }

            Log.StreamProcessorSuccessfullyRegistered(_logger, streamProcessorId);
            return streamProcessor;
        }
        catch (Exception ex)
        {
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
        StreamProcessor existing;
        do
        {
            _streamProcessors.TryRemove(id, out existing);
        }
        while (existing != default);
        Log.StreamProcessorUnregistered(_logger, id);
    }
}
