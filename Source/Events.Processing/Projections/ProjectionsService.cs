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
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Projections.Store.Definition;

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
        readonly ICompareProjectionDefinitionsForAllTenants _projectionDefinitionComparer;
        readonly FactoryFor<IProjectionStates> _getProjectionStates;
        readonly FactoryFor<IProjectionDefinitions> _getProjectionDefinitions;
        readonly IProjectionKeys _projectionKeys;
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
        /// <param name="projectionDefinitionComparer">The <see cref="ICompareProjectionDefinitionsForAllTenants" />.</param>
        /// <param name="getProjectionStates">The <see cref="FactoryFor{T}" /> for <see cref="IProjectionStates" />.</param>
        /// <param name="getProjectionDefinitions">The <see cref="FactoryFor{T}" /> for <see cref="IProjectionDefinitions" />.</param>
        /// <param name="projectionKeys">The <see cref="IProjectionKeys" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public ProjectionsService(
            IHostApplicationLifetime hostApplicationLifetime,
            IStreamProcessors streamProcessors,
            IExecutionContextManager executionContextManager,
            IInitiateReverseCallServices reverseCallServices,
            IProjectionsProtocol protocol,
            ICompareProjectionDefinitionsForAllTenants projectionDefinitionComparer,
            FactoryFor<IProjectionStates> getProjectionStates,
            FactoryFor<IProjectionDefinitions> getProjectionDefinitions,
            IProjectionKeys projectionKeys,
            ILoggerFactory loggerFactory)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _streamProcessors = streamProcessors;
            _executionContextManager = executionContextManager;
            _reverseCallServices = reverseCallServices;
            _protocol = protocol;
            _projectionDefinitionComparer = projectionDefinitionComparer;
            _getProjectionStates = getProjectionStates;
            _getProjectionDefinitions = getProjectionDefinitions;
            _projectionKeys = projectionKeys;
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
            _logger.ReceivedProjection(arguments.ProjectionDefinition.Projection.Value, arguments.ProjectionDefinition.Scope);

            _logger.LogDebug("Connecting Projection '{ProjectionId}'", arguments.ProjectionDefinition.Projection.Value);

            var tryRegisterEventProcessorStreamProcessor = TryRegisterEventProcessorStreamProcessor(
                arguments.ProjectionDefinition.Scope,
                arguments.ProjectionDefinition.Projection.Value,
                () => new EventProcessor(
                    arguments.ProjectionDefinition,
                    dispatcher,
                    _getProjectionStates(),
                    _projectionKeys,
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
                    _logger.ProjectionAlreadyRegistered(arguments.ProjectionDefinition.Projection.Value);
                    var failure = new Failure(
                        ProjectionFailures.FailedToRegisterProjection,
                        $"Failed to register Projection: {arguments.ProjectionDefinition.Projection.Value}. Event Processor already registered on Source Stream: '{arguments.ProjectionDefinition.Projection.Value}'");
                    await dispatcher.Reject(new ProjectionRegistrationResponse { Failure = failure }, cts.Token).ConfigureAwait(false);
                    return;
                }
            }

            using var eventProcessorStreamProcessor = tryRegisterEventProcessorStreamProcessor.Result;

            await ResetIfDefinitionChanged(arguments.ProjectionDefinition, cts.Token).ConfigureAwait(false);

            var tryStartEventHandler = await TryStartProjection(
                dispatcher,
                arguments.ProjectionDefinition.Scope,
                arguments.ProjectionDefinition.Projection.Value,
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
                    _logger.CouldNotStartProjection(arguments.ProjectionDefinition.Projection.Value, arguments.ProjectionDefinition.Scope);
                    return;
                }
            }

            var tasks = tryStartEventHandler.Result;
            try
            {
                await Task.WhenAny(tasks).ConfigureAwait(false);

                if (tasks.TryGetFirstInnerMostException(out var ex))
                {
                    _logger.ErrorWhileRunningProjection(ex, arguments.ProjectionDefinition.Projection.Value, arguments.ProjectionDefinition.Scope);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            finally
            {
                cts.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _logger.ProjectionDisconnected(arguments.ProjectionDefinition.Projection.Value, arguments.ProjectionDefinition.Scope);
            }
        }

        async Task ResetIfDefinitionChanged(ProjectionDefinition projectionDefinition, CancellationToken token)
        {
            _logger.LogDebug(
                "Comparing projection definition for projection {Projection} in scope {Scope}",
                projectionDefinition.Projection.Value,
                projectionDefinition.Scope.Value);
            var comparisonResults = await _projectionDefinitionComparer.DiffersFromPersisted(projectionDefinition, token).ConfigureAwait(false);
            var executionContext = _executionContextManager.Current;
            foreach (var (tenant, result) in comparisonResults)
            {
                _logger.LogDebug("Persisting projections for tenant {Tenant}", tenant.Value);
                await _getProjectionDefinitions().TryPersist(projectionDefinition, token).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    _logger.LogDebug("Resetting projections for tenant {Tenant} because: {Reason}", tenant.Value, result.FailureReason);
                    await _getProjectionStates().TryDrop(projectionDefinition.Projection, projectionDefinition.Scope, token).ConfigureAwait(false);
                }
            }
            _executionContextManager.CurrentFor(executionContext);
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
