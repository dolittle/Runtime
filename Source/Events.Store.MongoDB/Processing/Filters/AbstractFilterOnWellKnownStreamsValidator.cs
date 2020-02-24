// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="ICanFetchEventsFromWellKnownStreams" />.
    /// </summary>
    public abstract class AbstractFilterOnWellKnownStreamsValidator : ICanValidateFilterOnWellKnownStreams
    {
        readonly EventStoreConnection _connection;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterOnWellKnownStreamsValidator"/> class.
        /// </summary>
        /// <param name="streams">The streams it can fetch from.</param>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        protected AbstractFilterOnWellKnownStreamsValidator(IEnumerable<StreamId> streams, IFetchEventTypesFromStreams eventTypesFromStreams, EventStoreConnection connection)
        {
            WellKnownStreams = streams;
            _eventTypesFromStreams = eventTypesFromStreams;
            _connection = connection;
        }

        /// <inheritdoc/>
        public IEnumerable<StreamId> WellKnownStreams { get; }

        /// <inheritdoc/>
        public bool CanValidateFilterOn(StreamId sourceStream) => WellKnownStreams.Contains(sourceStream);

        /// <inheritdoc/>
        public abstract Task Validate(StreamId sourceStream, StreamId targetStream, AbstractFilterProcessor filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the last event log version number in this stream.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The last event log version.</returns>
        protected async Task<EventLogVersion> GetLastEventLogVersionInStream(StreamId stream, CancellationToken cancellationToken)
        {
            try
            {
                var events = await _connection.GetStreamCollectionAsync(stream, cancellationToken).ConfigureAwait(false);
                var version = await events
                    .Find(FilterDefinition<Events.StreamEvent>.Empty)
                    .Sort(Builders<Events.StreamEvent>.Sort.Descending(_ => _.Metadata.EventLogVersion))
                    .Project(Builders<Events.StreamEvent>.Projection.Expression(_ => _.Metadata.EventLogVersion))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                return version;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <summary>
        /// Gets all the event <see cref="Artifact" /> artifacts in this stream.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>All the event artifacts.</returns>
        protected Task<IEnumerable<Artifact>> GetAllEventArtifactsInStream(StreamId stream, CancellationToken cancellationToken) => _eventTypesFromStreams.FetchTypesInRange(stream, StreamPosition.Start, uint.MaxValue, cancellationToken);
    }
}