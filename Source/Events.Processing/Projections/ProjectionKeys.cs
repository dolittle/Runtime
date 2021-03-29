// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionKeys" />.
    /// </summary>
    [Singleton]
    public class ProjectionKeys : IProjectionKeys
    {
        readonly IProjectionKeyPropertyExtractor _keyPropertyExtractor;

        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionKeys" /> class.
        /// </summary>
        /// <param name="keyPropertyExtractor">The projection key property extractor.</param>
        public ProjectionKeys(IProjectionKeyPropertyExtractor keyPropertyExtractor)
        {
            _keyPropertyExtractor = keyPropertyExtractor;
        }

        public bool TryGetFor(ProjectionDefinition projectionDefinition, CommittedEvent @event, PartitionId partition, out ProjectionKey key)
        {
            key = null;
            var eventSelector = projectionDefinition.Events.FirstOrDefault(_ => _.EventType == @event.Type);
            if (eventSelector == null) return false;
            return TryGetKey(eventSelector, @event, partition, out key);
        }

        bool TryGetKey(ProjectionEventSelector eventSelector, CommittedEvent @event, PartitionId partition, out ProjectionKey key)
            => PartitionIsKey(eventSelector.KeySelectorType, partition, out key)
                || EventSourceIsKey(eventSelector.KeySelectorType, @event.EventSource, out key)
                || PropertyIsKey(eventSelector.KeySelectorType, @event.Content, eventSelector.KeySelectorExpression, out key);

        bool PartitionIsKey(ProjectEventKeySelectorType type, PartitionId partition, out ProjectionKey key)
        {
            key = null;
            if (type == ProjectEventKeySelectorType.PartitionId)
            {
                key = partition.Value.ToString();
                return true;
            }
            return false;
        }

        bool EventSourceIsKey(ProjectEventKeySelectorType type, EventSourceId eventSource, out ProjectionKey key)
        {
            key = null;
            if (type == ProjectEventKeySelectorType.EventSourceId)
            {
                key = eventSource.Value.ToString();
                return true;
            }
            return false;
        }

        bool PropertyIsKey(ProjectEventKeySelectorType type, string eventContent, KeySelectorExpression keySelectorExpression, out ProjectionKey key)
        {
            key = null;
            if (type != ProjectEventKeySelectorType.Property) return false;
            return _keyPropertyExtractor.TryExtract(eventContent, keySelectorExpression, out key);
        }
    }
}
