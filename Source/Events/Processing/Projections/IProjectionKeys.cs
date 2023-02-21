// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Defines a system that can retrieve a <see cref="ProjectionKey" /> from a projection.
/// </summary>
public interface IProjectionKeys
{
    /// <summary>
    /// Try to get the <see cref="ProjectionKey" />.
    /// </summary>
    /// <param name="projectionDefinition">The <see cref="ProjectionDefinition" />.</param>
    /// <param name="event">The <see cref="CommittedEvent" /> to map to a projection state.</param>
    /// <param name="partition">The <see cref="PartitionId" /> that the event belongs to in the stream.</param>
    /// <param name="key">The <see cref="ProjectionKey" />.</param>
    bool TryGetFor(ProjectionDefinition projectionDefinition, CommittedEvent @event, PartitionId partition, out ProjectionKey key);
}