
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.Persistence;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Represents an implementation of the <see cref="IUpdateAggregateVersionsAfterCommit"/>.
/// </summary>
[PerTenant]
public class UpdateAggregateVersionsAfterCommit : IUpdateAggregateVersionsAfterCommit
{
    readonly IAggregateRoots _aggregateRoots;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAggregateVersionsAfterCommit"/> class.
    /// </summary>
    /// <param name="aggregateRoots"></param>
    public UpdateAggregateVersionsAfterCommit(IAggregateRoots aggregateRoots)
    {
        _aggregateRoots = aggregateRoots;
    }

    /// <inheritdoc />
    public async Task UpdateAggregateVersions(IClientSessionHandle session, Commit commit, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        var batchedCommittedAggregateEvents = commit.AggregateEvents.ToArray();
        if (batchedCommittedAggregateEvents.Length == 0)
        {
            return;
        }
        foreach (var committedAggregateEvents in batchedCommittedAggregateEvents)
        {
            var expectedVersion = committedAggregateEvents[0].AggregateRootVersion;
            AggregateRootVersion nextVersion = expectedVersion + (ulong)committedAggregateEvents.Count;
            tasks.Add(_aggregateRoots.IncrementVersionFor(session, committedAggregateEvents.EventSource, committedAggregateEvents.AggregateRoot, expectedVersion, nextVersion, cancellationToken));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
