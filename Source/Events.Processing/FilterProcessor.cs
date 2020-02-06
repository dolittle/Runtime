// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Logging;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a default implementation of <see cref="AbstractFilterProcessor"/>.
    /// </summary>
    public class FilterProcessor : AbstractFilterProcessor
    {
        readonly IAsyncStreamReader<FilterClientToRuntimeResponse> _requestStream;
        readonly IServerStreamWriter<FilterRuntimeToClientRequest> _responseStream;
        readonly ServerCallContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessor"/> class.
        /// </summary>
        /// <param name="requestStream"><see cref="IAsyncStreamReader{T}"/> for client responses.</param>
        /// <param name="responseStream"><see cref="IServerStreamWriter{T}"/> for server requests.</param>
        /// <param name="context"><see cref="ServerCallContext"/> for the processor.</param>
        /// <param name="targetStreamId"><see cref="StreamId"/> to write to after filtering.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FilterProcessor(
            IAsyncStreamReader<FilterClientToRuntimeResponse> requestStream,
            IServerStreamWriter<FilterRuntimeToClientRequest> responseStream,
            ServerCallContext context,
            StreamId targetStreamId,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
            : base(targetStreamId, eventsToStreamsWriter, logger)
        {
            _requestStream = requestStream;
            _responseStream = responseStream;
            _context = context;
        }

        /// <inheritdoc/>
        public override async Task<IFilterResult> Filter(Store.CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return new SucceededFilteringResult(true, partitionId);
        }
    }
}