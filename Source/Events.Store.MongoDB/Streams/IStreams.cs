// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Defines a system that knows Streams.
/// </summary>
public interface IStreams : IEventStoreConnection
{
    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}" /> of <see cref="MongoDB.Events.Event" />.
    /// </summary>
    IMongoCollection<MongoDB.Events.Event> DefaultEventLog { get; }

    /// <summary>
    /// Gets a non-public and non-event-log Stream.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="streamId">The <see cref="StreamId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="MongoDB.Events.StreamEvent" />.</returns>
    Task<IMongoCollection<MongoDB.Events.StreamEvent>> Get(ScopeId scopeId, StreamId streamId, CancellationToken token);

    /// <summary>
    /// Gets a Stream.
    /// </summary>
    /// <param name="streamId">The <see cref="StreamId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="MongoDB.Events.StreamEvent" />.</returns>
    Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetPublic(StreamId streamId, CancellationToken token);

    /// <summary>
    /// Gets a Stream Definition collection.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="MongoDB.Streams.StreamDefinition" />.</returns>
    Task<IMongoCollection<MongoDB.Streams.StreamDefinition>> GetDefinitions(ScopeId scopeId, CancellationToken token);

    /// <summary>
    /// Gets an Event Log for a given <see cref="ScopeId" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="MongoDB.Events.Event" />.</returns>
    Task<IMongoCollection<MongoDB.Events.Event>> GetEventLog(ScopeId scopeId, CancellationToken token);
}