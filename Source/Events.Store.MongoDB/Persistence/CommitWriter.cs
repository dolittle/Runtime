// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Represents an implementation of <see cref="IPersistCommits"/>.
/// </summary>
[PerTenant]
public class CommitWriter : IPersistCommits
{
    readonly IStreams _streams;
    // readonly IStreamEventWatcher _streamWatcher;
    readonly IConvertCommitToEvents _commitConverter;
    readonly IUpdateAggregateVersionsAfterCommit _aggregateVersions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommitWriter"/> class. 
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher"/>.</param>
    /// <param name="commitConverter">The <see cref="IConvertCommitToEvents"/>.</param>
    public CommitWriter(IStreams streams, IStreamEventWatcher streamWatcher, IConvertCommitToEvents commitConverter, IUpdateAggregateVersionsAfterCommit aggregateVersions)
    {
        _streams = streams;
        // _streamWatcher = streamWatcher;
        _commitConverter = commitConverter;
        _aggregateVersions = aggregateVersions;
    }

    /// <inheritdoc />
    public async Task<Try> Persist(Commit commit, CancellationToken cancellationToken)
    {
        var eventsToStore = _commitConverter.ToEvents(commit).ToArray();
        if (eventsToStore.Length == 0)
        {
            return new NoEventsToCommit();
        }
        using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        try
        {
            session.StartTransaction();
            await _streams.DefaultEventLog.InsertManyAsync(
                session,
                eventsToStore,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            await _aggregateVersions.UpdateAggregateVersions(session, commit, cancellationToken).ConfigureAwait(false);
            await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            //TODO: Notifying for events should be a concern handled by actors
            // _streamWatcher.NotifyForEvent(ScopeId.Default, StreamId.EventLog, eventsToStore.Max(_ => _.EventLogSequenceNumber));
            return Try.Succeeded;
        }
        catch (MongoWaitQueueFullException ex)
        {
            await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
            return new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
            return ex;
        }
        
    }
}
