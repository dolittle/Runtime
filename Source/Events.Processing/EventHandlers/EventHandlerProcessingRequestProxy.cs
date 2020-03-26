// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="ProcessingRequestProxy{TRequest}" /> for <see cref="EventHandlerRuntimeToClientRequest" />.
    /// </summary>
    public class EventHandlerProcessingRequestProxy : ProcessingRequestProxy<EventHandlerRuntimeToClientRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProcessingRequestProxy"/> class.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext "/>.</param>
        public EventHandlerProcessingRequestProxy(CommittedEvent @event, PartitionId partition, ExecutionContext executionContext)
            : base(@event, partition, executionContext)
        {
        }

        /// <inheritdoc/>
        public override EventHandlerRuntimeToClientRequest ToProcessingRequest() =>
            new EventHandlerRuntimeToClientRequest { Event = Event.ToProtobuf(), Partition = Partition.ToProtobuf(), ExecutionContext = ExecutionContext.ToByteString() };
    }
}