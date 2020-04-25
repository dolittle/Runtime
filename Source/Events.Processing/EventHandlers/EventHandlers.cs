// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHandlers" />.
    /// </summary>
    [SingletonPerTenant]
    public class EventHandlers : IEventHandlers
    {
        readonly IStreamProcessors _streamProcessors;
        readonly IWriteEventsToStreams _eventsToStreamsWriter;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IFilterValidators _filterValidators;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="streamProcessors"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriter"><see cref="FactoryFor{T}"/> the  <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="eventsFromStreamsFetcher"><see cref="FactoryFor{T}"/> the  <see cref="IFetchEventsFromStreams">fetcher</see> for writing events.</param>
        /// <param name="filterValidators">The <see cref="FilterValidators" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlers(
            IStreamProcessors streamProcessors,
            IWriteEventsToStreams eventsToStreamsWriter,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            IFilterValidators filterValidators,
            ILogger logger)
        {
            _streamProcessors = streamProcessors;
            _eventsToStreamsWriter = eventsToStreamsWriter;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _filterValidators = filterValidators;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<EventHandlerRegistrationResult> Register(
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken)
        {
            StreamId targetStream = eventProcessor.Identifier.Value;

            var filter = new TypeFilterWithEventSourcePartition(eventProcessor.Scope, filterDefinition, _eventsToStreamsWriter, _logger);
            var eventHandlerRegistrationResult = _streamProcessors.Register(eventProcessor, _eventsFromStreamsFetcher, targetStream, cancellationToken);
            var filterRegistrationResult = _streamProcessors.Register(filter, _eventsFromStreamsFetcher, filterDefinition.SourceStream, cancellationToken);
            var filterValidationResult = await _filterValidators.Validate(filter, cancellationToken).ConfigureAwait(false);
            var failureReason = FailedEventHandlerRegistrationReason.FromRegistrationResults(eventHandlerRegistrationResult, filterRegistrationResult, filterValidationResult);

            return failureReason.IsSet switch
                {
                    true => new EventHandlerRegistrationResult(failureReason),
                    _ => new EventHandlerRegistrationResult(filterRegistrationResult.StreamProcessor, eventHandlerRegistrationResult.StreamProcessor, filter)
                };
        }
    }
}