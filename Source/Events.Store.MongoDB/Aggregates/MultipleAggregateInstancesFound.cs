// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates;

/// <summary>
/// Exception that gets thrown when multiple versions of a single aggregate root instance is found in the MongoDB.
/// </summary>
public class MultipleAggregateInstancesFound : EventStoreConsistencyError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleAggregateInstancesFound"/> class.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> of the aggregate root instance.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> of the aggregate root instance.</param>
    public MultipleAggregateInstancesFound(EventSourceId eventSource, ArtifactId aggregateRoot)
        : base($"Multiple versions of a single aggregate root instance with event source id '{eventSource}' and aggregate root id '{aggregateRoot}'.", null)
    {
    }
}