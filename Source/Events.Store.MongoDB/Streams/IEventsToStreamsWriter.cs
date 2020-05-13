// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Defines a that can write events to a Stream.
    /// </summary>
    public interface IEventsToStreamsWriter
    {
        /// <summary>
        /// Writes an <typeparamref name="TEvent">Event</typeparamref> to <see cref="IMongoCollection{TDocument}" /> Stream.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="stream">The Stream <see cref="IMongoCollection{TDocument}" />.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
        /// <param name="createStoreEvent">A <see cref="Func{T, TResult}" /> that creates a <typeparamref name="TEvent" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Write<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Func<StreamPosition, TEvent> createStoreEvent,
            CancellationToken cancellationToken)
            where TEvent : class;
    }
}
