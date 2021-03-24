// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.Projections;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents the implementation of <see cref="ProjectionsBase"/>.
    /// </summary>
    public class ProjectionsService : ProjectionsBase
    {
        readonly IStreamProcessors _streamProcessors;
        readonly IExecutionContextManager _executionContextManager;
        readonly IInitiateReverseCallServices _reverseCallServices;
        readonly IProjectionsProtocol _protocol;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="reverseCallServices">The <see cref="IInitiateReverseCallServices" />.</param>
        /// <param name="protocol">The <see cref="IProjectionsProtocol" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public ProjectionsService(
            IHostApplicationLifetime hostApplicationLifetime,
            IStreamProcessors streamProcessors,
            IExecutionContextManager executionContextManager,
            IInitiateReverseCallServices reverseCallServices,
            IProjectionsProtocol protocol,
            ILoggerFactory loggerFactory)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _streamProcessors = streamProcessors;
            _executionContextManager = executionContextManager;
            _reverseCallServices = reverseCallServices;
            _protocol = protocol;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ProjectionsService>();
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<ProjectionClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<ProjectionRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.LogDebug("Connecting Projections");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var tryConnect = await _reverseCallServices.Connect(
                runtimeStream,
                clientStream,
                context,
                _protocol,
                cts.Token).ConfigureAwait(false);
            if (!tryConnect.Success) return;
            var (dispatcher, arguments) = tryConnect.Result;
            _logger.SettingExecutionContext(arguments.ExecutionContext);
            _executionContextManager.CurrentFor(arguments.ExecutionContext);
            _logger.ReceivedProjection(arguments.Projection, arguments.Scope);

            _logger.LogDebug("Connecting Projection '{ProjectionId}'", arguments.Projection.Value);

            var tryRegisterEventProcessorStreamProcessor = TryRegisterEventProcessorStreamProcessor(
                arguments.Scope,
                arguments.Projection,
                () => new EventProcessor(
                    arguments.Scope,
                    arguments.Projection,
                    dispatcher,
                    _loggerFactory.CreateLogger<EventProcessor>()),
                cts.Token);

            if (!tryRegisterEventProcessorStreamProcessor.Success)
            {
                if (tryRegisterEventProcessorStreamProcessor.HasException)
                {
                    var exception = tryRegisterEventProcessorStreamProcessor.Exception;
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.ProjectionAlreadyRegistered(arguments.Projection);
                    var failure = new Failure(
                        ProjectionFailures.FailedToRegisterProjection,
                        $"Failed to register Projection: {arguments.Projection.Value}. Event Processor already registered on Source Stream: '{arguments.Projection.Value}'");
                    await dispatcher.Reject(new ProjectionRegistrationResponse { Failure = failure }, cts.Token).ConfigureAwait(false);
                    return;
                }
            }

            using var eventProcessorStreamProcessor = tryRegisterEventProcessorStreamProcessor.Result;

            var tryStartEventHandler = await TryStartProjection(
                dispatcher,
                arguments.Scope,
                arguments.Projection,
                eventProcessorStreamProcessor,
                cts.Token).ConfigureAwait(false);
            if (!tryStartEventHandler.Success)
            {
                cts.Cancel();
                if (tryStartEventHandler.HasException)
                {
                    var exception = tryStartEventHandler.Exception;
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.CouldNotStartProjection(arguments.Projection, arguments.Scope);
                    return;
                }
            }

            var tasks = tryStartEventHandler.Result;
            try
            {
                await Task.WhenAny(tasks).ConfigureAwait(false);

                if (tasks.TryGetFirstInnerMostException(out var ex))
                {
                    _logger.ErrorWhileRunningProjection(ex, arguments.Projection, arguments.Scope);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            finally
            {
                cts.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _logger.ProjectionDisconnected(arguments.Projection, arguments.Scope);
            }
        }

        async Task<Try<IEnumerable<Task>>> TryStartProjection(
            IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> dispatcher,
            ScopeId scopeId,
            EventProcessorId projectionId,
            StreamProcessor eventProcessorStreamProcessor,
            CancellationToken cancellationToken)
        {
            _logger.StartingProjection(projectionId);
            try
            {
                var runningDispatcher = dispatcher.Accept(new ProjectionRegistrationResponse(), cancellationToken);
                await eventProcessorStreamProcessor.Initialize().ConfigureAwait(false);
                return new[] { eventProcessorStreamProcessor.Start(), runningDispatcher };
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileStartingProjection(ex, projectionId, scopeId);
                }

                return ex;
            }
        }

        Try<StreamProcessor> TryRegisterEventProcessorStreamProcessor(
            ScopeId scopeId,
            EventProcessorId projectionId,
            Func<IEventProcessor> getEventProcessor,
            CancellationToken cancellationToken)
        {
            _logger.RegisteringStreamProcessorForEventProcessor(projectionId, StreamId.EventLog);
            try
            {
                return (_streamProcessors.TryRegister(
                    scopeId,
                    projectionId,
                    new EventLogStreamDefinition(),
                    getEventProcessor,
                    cancellationToken,
                    out var outputtedEventProcessorStreamProcessor), outputtedEventProcessorStreamProcessor);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileRegisteringStreamProcessorForEventProcessor(ex, projectionId);
                }

                return ex;
            }
        }
    }
}
