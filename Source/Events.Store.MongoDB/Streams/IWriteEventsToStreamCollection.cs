// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Defines a that can write events to a Stream <see cref="IMongoCollection{TDocument}" />.
/// </summary>
public interface IWriteEventsToStreamCollection
{
    /// <summary>
    /// Writes an <typeparamref name="TEvent">Event</typeparamref> to <see cref="IMongoCollection{TDocument}" /> Stream collection.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="stream">The Stream <see cref="IMongoCollection{TDocument}" />.</param>
    /// <param name="createStoreEvent">A <see cref="Func{T, TResult}" /> that creates a <typeparamref name="TEvent" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task<StreamPosition> Write<TEvent>(
        IMongoCollection<TEvent> stream,
        Func<StreamPosition, TEvent> createStoreEvent,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Writes multiple <typeparamref name="TEvent">Event</typeparamref> to <see cref="IMongoCollection{TDocument}" /> Stream collection.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="stream">The Stream <see cref="IMongoCollection{TDocument}" />.</param>
    /// <param name="createStoreEvents">A <see cref="Func{T, TResult}" /> that creates a <typeparamref name="TEvent" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task<StreamPosition> Write<TEvent>(
        IMongoCollection<TEvent> stream,
        Func<StreamPosition, IReadOnlyList<TEvent>> createStoreEvents,
        CancellationToken cancellationToken);
}
