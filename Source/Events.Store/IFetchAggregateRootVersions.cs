// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines a system that can fetch Aggregate Root Instances from the Event Store.
/// </summary>
public interface IFetchAggregateRootVersions
{
    /// <summary>
    /// Fetches the current version of the aggregate root instance in the event store.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The current <see cref="AggregateRootVersion" /> for an aggregate root instance.</returns>
    Task<AggregateRootVersion> FetchVersionFor(EventSourceId eventSource, ArtifactId aggregateRoot, CancellationToken cancellationToken);
}
