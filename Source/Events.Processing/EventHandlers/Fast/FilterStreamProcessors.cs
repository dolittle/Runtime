// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IStreamProcessors" />.
/// </summary>
[Singleton]
public class FilterStreamProcessors : IFilterStreamProcessors
{
    readonly Func<StreamProcessorId, TypeFilterWithEventSourcePartitionDefinition, Action, CancellationToken, FilterStreamProcessor> _createStreamProcessor;
    readonly ILogger _logger;
    readonly ConcurrentDictionary<StreamProcessorId, FilterStreamProcessor> _streamProcessors = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
    /// </summary>
    public FilterStreamProcessors(
        Func<StreamProcessorId, TypeFilterWithEventSourcePartitionDefinition, Action, CancellationToken, FilterStreamProcessor> createStreamProcessor,
        ILogger logger)
    {
        _createStreamProcessor = createStreamProcessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public Try<FilterStreamProcessor> TryCreateAndRegister(
        ScopeId scopeId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        CancellationToken cancellationToken)
    {
        try
        {
            
            var streamProcessorId = new StreamProcessorId(scopeId, filterDefinition.TargetStream.Value, StreamId.EventLog);
            if (_streamProcessors.ContainsKey(streamProcessorId))
            {
                _logger.FilterStreamProcessorAlreadyRegistered(streamProcessorId);
                return new StreamProcessorAlreadyRegistered(streamProcessorId);
            }
            
            var streamProcessor = _createStreamProcessor(
                streamProcessorId,
                filterDefinition,
                () => Unregister(streamProcessorId),
                cancellationToken);
            
            if (!_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
            {
                _logger.FilterStreamProcessorAlreadyRegistered(streamProcessorId);
                return new StreamProcessorAlreadyRegistered(streamProcessorId);
            }

            _logger.FilterStreamProcessorSuccessfullyRegistered(streamProcessorId);
            return streamProcessor;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    void Unregister(StreamProcessorId id)
    {
        FilterStreamProcessor existing;
        do
        {
            _streamProcessors.TryRemove(id, out existing);
        }
        while (existing != default);
        _logger.FilterStreamProcessorUnregistered(id);
    }
}
