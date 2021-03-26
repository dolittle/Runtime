// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionKeys" />.
    /// </summary>
    [Singleton]
    public class ProjectionKeys : IProjectionKeys
    {
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionKeys" /> class.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        public ProjectionKeys(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public bool TryGetFor(ProjectionDefinition projectionDefinition, CommittedEvent @event, PartitionId partition, out ProjectionKey key)
        {
            key = null;
            var eventSelector = projectionDefinition.Events.FirstOrDefault(_ => _.EventType == @event.Type);
            if (eventSelector == null) return false;
            if (TryGetKey(eventSelector, @event, partition, out key)) return true;
            return false;
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
            if (type == ProjectEventKeySelectorType.Property)
            {
                var eventContentKeyValues = _serializer.GetKeyValuesFromJson(eventContent);
                if (!eventContentKeyValues.ContainsKey(keySelectorExpression)) return false;

                key = eventContentKeyValues[keySelectorExpression].ToString(); // Todo: What to do about this?
                return true;
            }
            return false;
        }
    }
}