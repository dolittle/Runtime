// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes sends and event to other microservices.
    /// </summary>
    public class EventHorizon : IEventProcessor
    {
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizon"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHorizon(ILogger logger)
        {
            _logger = logger;
            _logMessagePrefix = $"Event Processor '{Identifier}'";
        }

        /// <inheritdoc />
        public EventProcessorId Identifier => StreamId.PublicEventsId.Value;

        /// <inheritdoc />
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId)
        {
            _logger.Debug($"{_logMessagePrefix} is processing public event '{@event.Type.Id.Value}''");

            return Task.FromResult<IProcessingResult>(new SucceededProcessingResult());
        }
    }
}