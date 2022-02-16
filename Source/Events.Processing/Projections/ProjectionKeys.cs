// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Linq;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="IProjectionKeys" />.
/// </summary>
[Singleton]
public class ProjectionKeys : IProjectionKeys
{
    readonly IProjectionKeyPropertyExtractor _keyPropertyExtractor;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes an instance of the <see cref="ProjectionKeys" /> class.
    /// </summary>
    /// <param name="keyPropertyExtractor">The projection key property extractor.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public ProjectionKeys(IProjectionKeyPropertyExtractor keyPropertyExtractor, ILogger logger)
    {
        _keyPropertyExtractor = keyPropertyExtractor;
        _logger = logger;
    }

    public bool TryGetFor(ProjectionDefinition projectionDefinition, CommittedEvent @event, PartitionId partition, out ProjectionKey key)
    {
        key = null;
        var eventSelector = projectionDefinition.Events.FirstOrDefault(_ => _.EventType == @event.Type.Id);
        return eventSelector != null && TryGetKey(eventSelector, @event, partition, out key);
    }

    bool TryGetKey(ProjectionEventSelector eventSelector, CommittedEvent @event, PartitionId partition, out ProjectionKey key)
        => PartitionIsKey(eventSelector.KeySelectorType, partition, out key)
            || EventSourceIsKey(eventSelector.KeySelectorType, @event.EventSource, out key)
            || PropertyIsKey(eventSelector.KeySelectorType, @event.Content, eventSelector.KeySelectorExpression, out key)
            || StaticIsKey(eventSelector.KeySelectorType, eventSelector.StaticKey, out key)
            || OccurredIsKey(eventSelector.KeySelectorType, eventSelector.OccurredFormat, @event.Occurred, out key);

    bool OccurredIsKey(ProjectEventKeySelectorType type, OccurredFormat occurredFormat, DateTimeOffset eventOccurred, out ProjectionKey key)
    {
        key = null;
        if (type != ProjectEventKeySelectorType.EventOccurred)
        {
            return false;
        }
        try
        {
            key = eventOccurred.ToString(occurredFormat, CultureInfo.InvariantCulture);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get projection key for event occurred key selector type with occurred format {OccurredFormat}", occurredFormat);
            key = null;
            return false;
        }
    }

    static bool StaticIsKey(ProjectEventKeySelectorType type, ProjectionKey staticKey, out ProjectionKey key)
    {
        key = null;
        if (type != ProjectEventKeySelectorType.Static)
        {
            return false;
        }
        key = staticKey;
        return true;
    }

    static bool PartitionIsKey(ProjectEventKeySelectorType type, PartitionId partition, out ProjectionKey key)
    {
        key = null;
        if (type != ProjectEventKeySelectorType.PartitionId)
        {
            return false;
        }
        key = partition.Value;
        return true;
    }

    static bool EventSourceIsKey(ProjectEventKeySelectorType type, EventSourceId eventSource, out ProjectionKey key)
    {
        key = null;
        if (type != ProjectEventKeySelectorType.EventSourceId)
        {
            return false;
        }
        key = eventSource.Value;
        return true;
    }

    bool PropertyIsKey(ProjectEventKeySelectorType type, string eventContent, KeySelectorExpression keySelectorExpression, out ProjectionKey key)
    {
        key = null;
        return type == ProjectEventKeySelectorType.Property && _keyPropertyExtractor.TryExtract(eventContent, keySelectorExpression, out key);
    }
}
