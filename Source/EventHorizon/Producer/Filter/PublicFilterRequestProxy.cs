// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Google.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Producer.Filter
{
    /// <summary>
    /// Represents an implementation of <see cref="ProcessingRequestProxy{TRequest}" /> for <see cref="PublicFilterRuntimeToClientRequest" />.
    /// </summary>
    public class PublicFilterRequestProxy : ProcessingRequestProxy<PublicFilterRuntimeToClientRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterRequestProxy"/> class.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext "/>.</param>
        public PublicFilterRequestProxy(CommittedEvent @event, PartitionId partition, ExecutionContext executionContext)
            : base(@event, partition, executionContext)
        {
        }

        /// <inheritdoc/>
        public override PublicFilterRuntimeToClientRequest ToProcessingRequest() =>
            new PublicFilterRuntimeToClientRequest { Event = Event.ToProtobuf(), Partition = Partition.ToProtobuf(), ExecutionContext = ExecutionContext.ToByteString() };
    }
}