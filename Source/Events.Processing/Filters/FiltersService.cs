// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IFilters _filters;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="filters">The <see cref="IFilters" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="eventsToStreamsWriterFactory">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FiltersService(
            IFilters filters,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            ILogger<FiltersService> logger)
        {
            _filters = filters;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _eventsToStreamsWriterFactory = eventsToStreamsWriterFactory;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task Connect(
            IAsyncStreamReader<FilterClientToRuntimeResponse> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientRequest> clientStream,
            ServerCallContext context)
        {
            var filterArguments = context.GetArgumentsMessage<FilterArguments>();
            var eventProcessorId = filterArguments.Filter.To<EventProcessorId>();
            var scope = filterArguments.Scope.To<ScopeId>();
            var streamId = StreamId.AllStreamId;

            var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                runtimeStream,
                clientStream,
                context,
                _ => _.CallNumber,
                _ => _.CallNumber);
            FilterProcessor createEventProcessor() => new FilterProcessor(
                scope,
                new RemoteFilterDefinition(streamId, eventProcessorId.Value),
                dispatcher,
                _eventsToStreamsWriterFactory(),
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