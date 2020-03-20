// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a proxy.
    /// </summary>
    /// <typeparam name="TRequest">The request <see cref="IMessage" /> type.</typeparam>
    public abstract class ProcessingRequestProxy<TRequest> : IProcessingRequest
        where TRequest : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingRequestProxy{T}"/> class.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        protected ProcessingRequestProxy(CommittedEvent @event, PartitionId partition, ExecutionContext executionContext)
        {
            Event = @event;
            Partition = partition;
            ExecutionContext = executionContext;
        }

        /// <inheritdoc/>
        public CommittedEvent Event { get; }

        /// <inheritdoc/>
        public PartitionId Partition { get; }

        /// <inheritdoc/>
        public ExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Converts the <see cre="ProcessingRequestProxy{T}" /> to the <see typeparam="TRequest" /> <see cref="IMessage" />.
        /// </summary>
        /// <param name="proxy">The <see cre="ProcessingRequestProxy{T}" />.</param>
        public static implicit operator TRequest(ProcessingRequestProxy<TRequest> proxy) => proxy.ToProcessingRequest();

        /// <summary>
        /// Converts the <see cref="ProcessingRequestProxy{T}" /> to the correct event processing request.
        /// </summary>
        /// <returns>The event processing request.</returns>
        public abstract TRequest ToProcessingRequest();
    }
}