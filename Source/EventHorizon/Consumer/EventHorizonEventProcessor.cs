// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />.
    /// </summary>
    public class EventHorizonEventProcessor : IEventProcessor
    {
        readonly EventHorizon _eventHorizon;
        readonly IWriteEventHorizonEvents _receivedEventsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonEventProcessor"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        /// <param name="receivedEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHorizonEventProcessor(
            ScopeId scope,
            EventHorizon eventHorizon,
            IWriteEventHorizonEvents receivedEventsWriter,
            ILogger logger)
        {
            Scope = scope;
            _eventHorizon = eventHorizon;
            _receivedEventsWriter = receivedEventsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public ScopeId Scope { get; }

        /// <inheritdoc/>
        public EventProcessorId Identifier => _eventHorizon.ProducerTenant.Value;

        /// <inheritdoc/>
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.Information($"Processing event '{@event.Type.Id}' in scope '{Scope}' from microservice '{_eventHorizon.ProducerMicroservice}' and tenant '{_eventHorizon.ProducerTenant}'");

            await _receivedEventsWriter.Write(@event, _eventHorizon, cancellationToken).ConfigureAwait(false);
            return new SucceededProcessingResult();
        }
    }
}