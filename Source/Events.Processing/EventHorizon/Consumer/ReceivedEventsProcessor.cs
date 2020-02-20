// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />.
    /// </summary>
    public class ReceivedEventsProcessor : IEventProcessor
    {
        readonly IWriteReceivedEvents _receivedEventsWriter;
        readonly Microservice _producerMicroservice;
        readonly TenantId _producerTenant;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEventsProcessor"/> class.
        /// </summary>
        /// <param name="producerMicroservice">The <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The <see cref="TenantId" />.</param>
        /// <param name="receivedEventsWriter">The <see cref="IWriteReceivedEvents" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ReceivedEventsProcessor(
            Microservice producerMicroservice,
            TenantId producerTenant,
            IWriteReceivedEvents receivedEventsWriter,
            ILogger logger)
        {
            _receivedEventsWriter = receivedEventsWriter;
            _producerMicroservice = producerMicroservice;
            _producerTenant = producerTenant;
            _logger = logger;
        }

        /// <inheritdoc/>
        public EventProcessorId Identifier => _producerTenant.Value;

        /// <inheritdoc/>
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.Information($"Processing event '{@event.Type.Id}' from microservice '{_producerMicroservice}' and tenant '{_producerTenant}'");

            await _receivedEventsWriter.Write(@event, _producerMicroservice, _producerTenant, cancellationToken).ConfigureAwait(false);
            return new SucceededProcessingResult();
        }
    }
}