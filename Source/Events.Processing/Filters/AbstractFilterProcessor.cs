// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the filtering of an event.
    /// </summary>
    /// <typeparam name="TDefinition">The <see cref="IFilterDefinition" />.</typeparam>
    public abstract class AbstractFilterProcessor<TDefinition> : IFilterProcessor<TDefinition>
        where TDefinition : IFilterDefinition
    {
        readonly IWriteEventsToStreams _eventsToStreamsWriter;
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterProcessor{T}"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="filterDefinition">The <see typeparam="TDefinition"/> <see cref="IFilterDefinition" /> for the filter processor.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="FactoryFor{IWriteEventsToStreams}" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        protected AbstractFilterProcessor(
            ScopeId scope,
            TDefinition filterDefinition,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
        {
            Scope = scope;
            Definition = filterDefinition;
            _eventsToStreamsWriter = eventsToStreamsWriter;
            _logger = logger;
            _logMessagePrefix = $"Filter Processor '{Identifier}' in scope '{Scope}' with source stream '{Definition.SourceStream}'";
        }

        /// <inheritdoc/>
        public ScopeId Scope { get; }

        /// <inheritdoc/>
        public TDefinition Definition { get; }

        /// <inheritdoc />
        public EventProcessorId Identifier => Definition.TargetStream.Value;

        /// <inheritdoc/>
        public abstract Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken);

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            _logger.Debug($"{_logMessagePrefix} is filtering event '{@event.Type.Id}' for partition '{partitionId}'");
            var result = await Filter(@event, partitionId, Identifier, cancellationToken).ConfigureAwait(false);
            _logger.Debug($"{_logMessagePrefix} filtered event '{@event.Type.Id}' for partition '{partitionId}' with result 'Succeeded' = {result.Succeeded}");

            if (result.Succeeded && result.IsIncluded)
            {
                _logger.Debug($"{_logMessagePrefix} writing event '{@event.Type.Id}' to stream '{Definition.TargetStream}' in partition '{partitionId}'");
                await _eventsToStreamsWriter.Write(@event, Scope, Definition.TargetStream, result.Partition, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }
    }
}