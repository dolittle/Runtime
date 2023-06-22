// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can fetch <see cref="ArtifactId">event types</see> from a partitioned <see cref="StreamId">streams</see>.
/// </summary>
public interface ICanFetchEventTypesFromPartitionedStream : ICanFetchEventTypesFromStream
{
    /// <summary>
    /// Fetch the unique <see cref="Artifact">event types</see> in a an inclusive range in a <see cref="StreamId" /> and <see cref="PartitionId" />.
    /// </summary>
    /// <param name="partitionId">The <see cref="PartitionId" />.</param>
    /// <param name="range">The <see cref="StreamPositionRange" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="ISet{Artifact}" /> event types.</returns>
    Task<ISet<Artifact>> FetchInRangeAndPartition(PartitionId partitionId, StreamPositionRange range, CancellationToken cancellationToken);
}