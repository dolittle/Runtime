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
    /// Represents an implementation of <see cref="IEventFetchers" />.
    /// </summary>
    public class EventFetchers : IEventFetchers
    {
        readonly EventStoreConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFetchers"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        public EventFetchers(EventStoreConnection connection)
        {
            _connection = connection;
        }
        public Task<ICanFetchEventsFromStream> GetFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICanFetchEventsFromStream> GetFetcherFor(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICanFetchRangeOfEventsFromStream> GetRangeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICanFetchRangeOfEventsFromStream> GetRangeFetcherFor(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICanFetchEventTypesFromStream> GetTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICanFetchEventTypesFromStream> GetTypeFetcherFor(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<StreamFetcher<TEvent>> CreateStreamFetcher<TEvent>(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
            where TEvent : class
        {

            if (streamId == StreamId.EventLog)
            {
                return new StreamFetcher<MongoDB.Events.Event>(
                    await _connection.GetEventLogCollection(scopeId, cancellationToken).ConfigureAwait(false),
                    Builders<MongoDB.Events.Event>.Filter,
                    _ => _.Metadata.
                )
            }
        }
    }
}
