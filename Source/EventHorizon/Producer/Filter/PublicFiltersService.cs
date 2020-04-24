// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.PublicFilters;

namespace Dolittle.Runtime.EventHorizon.Producer.Filter
{
    /// <summary>
    /// Represents the implementation of <see creF="PublicFiltersBase"/>.
    /// </summary>
    public class PublicFiltersService : PublicFiltersBase
    {
        readonly IFilters _filters;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFiltersService"/> class.
        /// </summary>
        /// <param name="filters">The <see cref="IFilters" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFiltersService(
            IFilters filters,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            ILogger<PublicFiltersService> logger)
        {
            _filters = filters;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task Connect(
            IAsyncStreamReader<PublicFilterClientToRuntimeResponse> runtimeStream,
            IServerStreamWriter<PublicFilterRuntimeToClientRequest> clientStream,
            ServerCallContext context)
        {
            var filterArguments = context.GetArgumentsMessage<FilterArguments>();
            var eventProcessorId = filterArguments.Filter.To<EventProcessorId>();
            var streamId = StreamId.AllStreamId;
            var scope = ScopeId.Default;
            var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                runtimeStream,
                clientStream,
                context,
                _ => _.CallNumber,
                _ => _.CallNumber);
            PublicFilterProcessor createEventProcessor() => new PublicFilterProcessor(
                new PublicFilterDefinition(streamId, eventProcessorId.Value),
                dispatcher,
                _getEventsToPublicStreamsWriter(),
                _executionContextManager,
                _logger);

            return _filters.RegisterAndStartProcessing(
                scope,
                eventProcessorId,
                streamId,
                dispatcher,
                createEventProcessor,
                context.CancellationToken);
        }
    }
}