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
using Dolittle.Types;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    public class FilterValidator : IFilterValidator
    {
        readonly IFilters _filters;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;
        readonly IEnumerable<ICanValidateFilterOnWellKnownStreams> _wellKnownValidators;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidator"/> class.
        /// </summary>
        /// <param name="filters">The <see cref="IFilters" />.</param>
        /// <param name="wellKnownValidators">The <see cref="IInstancesOf{T}" /> of <see cref="ICanValidateFilterOnWellKnownStreams" />.</param>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public FilterValidator(
            IFilters filters,
            IInstancesOf<ICanValidateFilterOnWellKnownStreams> wellKnownValidators,
            IFetchEventTypesFromStreams eventTypesFromStreams,
            EventStoreConnection connection,
            ILogger logger)
        {
            _wellKnownValidators = wellKnownValidators;
            _eventTypesFromStreams = eventTypesFromStreams;
            _connection = connection;
            _logger = logger;
            _filters = filters;
        }

        /// <inheritdoc/>
        public Task Validate(StreamId sourceStream, StreamId targetStream, CancellationToken cancellationToken = default)
        {
            var filter = _filters.GetFilterProcessorFor(targetStream);

            if (TryGetValidator(sourceStream, out var validator)) return validator.Validate(sourceStream, targetStream, filter, cancellationToken);
            return ValidateForStream(sourceStream, targetStream, filter, cancellationToken);
        }

        /// <inheritdoc/>
        public Task Validate(IFilterDefinition definition, CancellationToken cancellationToken = default) => Validate(definition.SourceStream, definition.TargetStream, cancellationToken);

        bool TryGetValidator(StreamId stream, out ICanValidateFilterOnWellKnownStreams validator)
        {
            validator = null;
            foreach (var instance in _wellKnownValidators)
            {
                if (instance.CanValidateFilterOn(stream))
                {
                    if (validator != null) throw new MultipleWellKnownFilterValidators(stream);
                    validator = instance;
                }
            }

            return validator != null;
        }

        async Task ValidateForStream(StreamId sourceStream, StreamId targetStream, AbstractFilterProcessor filter, CancellationToken cancellationToken)
        {
            try
            {
                var eventsInTargetStream = await _connection.GetStreamCollectionAsync(targetStream, cancellationToken).ConfigureAwait(false);
                var lastEventLogVersion = await eventsInTargetStream
                    .Find(FilterDefinition<Events.StreamEvent>.Empty)
                    .Sort(Builders<Events.StreamEvent>.Sort.Descending(_ => _.Metadata.EventLogVersion))
                    .Project(Builders<Events.StreamEvent>.Projection.Expression(_ => _.Metadata.EventLogVersion))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                var oldArtifacts = await _eventTypesFromStreams.FetchTypesInRange(targetStream, StreamPosition.Start, uint.MaxValue, cancellationToken).ConfigureAwait(false);
                var eventsInSourceStream = await _connection.GetStreamCollectionAsync(sourceStream, cancellationToken).ConfigureAwait(false);
                var committedEvents = await eventsInSourceStream
                    .Find(Builders<Events.StreamEvent>.Filter.Lte(_ => _.Metadata.EventLogVersion, lastEventLogVersion))
                    .Limit((int)lastEventLogVersion + 1)
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