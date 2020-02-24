// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractFilterOnWellKnownStreamsValidator" /> that can validate filters that are based off the public events stream.
    /// </summary>
    public class PublicEventsFilterValidator : AbstractFilterOnWellKnownStreamsValidator
    {
        readonly ILogger _logger;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;
        readonly EventStoreConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventsFilterValidator"/> class.
        /// </summary>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public PublicEventsFilterValidator(
            IFetchEventTypesFromStreams eventTypesFromStreams,
            EventStoreConnection connection,
            ILogger logger)
            : base(new StreamId[] { StreamId.PublicEventsId }, eventTypesFromStreams, connection)
        {
            _eventTypesFromStreams = eventTypesFromStreams;
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Validate(StreamId sourceStream, StreamId targetStream, AbstractFilterProcessor filter, CancellationToken cancellationToken)
        {
            try
            {
                var lastEventLogVersion = await GetLastEventLogVersionInStream(targetStream, cancellationToken).ConfigureAwait(false);
                var oldArtifacts = await GetAllEventArtifactsInStream(targetStream, cancellationToken).ConfigureAwait(false);
                var committedEvents = await _connection.PublicEvents
                    .Find(Builders<PublicEvent>.Filter.Lte(_ => _.Metadata.EventLogVersion, lastEventLogVersion))
                    .Limit((int)lastEventLogVersion.Value + 1)
                    .Project(_ => _.ToCommittedEvent()).ToListAsync(cancellationToken).ConfigureAwait(false);
                var newArtifacts = new List<Artifact>();
                foreach (var committedEvent in committedEvents)
                {
                    var filterResult = await filter.Filter(committedEvent, PartitionId.NotSet, targetStream.Value, cancellationToken).ConfigureAwait(false);
                    if (filterResult.Succeeded) newArtifacts.Add(committedEvent.Type);
                }

                FilteredStreamsValidator.ValidateArtifactsFromFilteredStreams(sourceStream, targetStream, oldArtifacts, newArtifacts);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}