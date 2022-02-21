// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System.Threading;
//using System.Threading.Tasks;
//using Dolittle.Runtime.Rudimentary;
//using Dolittle.Runtime.Events.Processing.Contracts;
//using Dolittle.Runtime.Execution;
//using Microsoft.Extensions.Logging;
//using Dolittle.Runtime.Protobuf;
//using Dolittle.Runtime.Services;
//using Dolittle.Runtime.Services.Hosting;
//using Grpc.Core;
//using Microsoft.Extensions.Hosting;
//using static Dolittle.Runtime.Events.Processing.Contracts.Projections;
//
//namespace Dolittle.Runtime.Events.Processing.Projections;
//
///// <summary>
///// Represents the implementation of <see cref="ProjectionsBase"/>.
///// </summary>
////TODO: [PrivateService]
//public class ProjectionsService : ProjectionsBase
//{
//    readonly IInitiateReverseCallServices _reverseCallServices;
//    readonly IExecutionContextManager _executionContextManager;
//    readonly IProjectionsProtocol _protocol;
//    readonly IProjections _projections;
//    readonly IHostApplicationLifetime _hostApplicationLifetime;
//    readonly ILogger _logger;
//
//    /// <summary>
//    /// Initializes a new instance of the <see cref="ProjectionsService"/> class.
//    /// </summary>
//    /// <param name="reverseCallServices">The initiator to use to start reverse call protocols.</param>
//    /// <param name="executionContextManager">The execution context manager to use to set the execution context from incoming requests.</param>
//    /// <param name="protocol">The projections protocol to use to parse and create messages.</param>
//    /// <param name="projections">The <see cref="IProjections"/> to use to register projections.</param>
//    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime"/> to use to stop ongoing reverse calls when the application is shutting down.</param>
//    /// <param name="logger">The logger to use for logging.</param>
//    public ProjectionsService(
//        IInitiateReverseCallServices reverseCallServices,
//        IExecutionContextManager executionContextManager,
//        IProjectionsProtocol protocol,
//        IProjections projections,
//        IHostApplicationLifetime hostApplicationLifetime,
//        ILogger logger)
//    {
//        _reverseCallServices = reverseCallServices;
//        _executionContextManager = executionContextManager;
//        _protocol = protocol;
//        _projections = projections;
//        _hostApplicationLifetime = hostApplicationLifetime;
//        _logger = logger;
//    }
//
//    /// <inheritdoc/>
//    public override async Task Connect(
//        IAsyncStreamReader<ProjectionClientToRuntimeMessage> runtimeStream,
//        IServerStreamWriter<ProjectionRuntimeToClientMessage> clientStream,
//        ServerCallContext context)
//    {
//        Log.ConnectingProjections(_logger);
//        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
//        var tryConnect = await _reverseCallServices.Connect(runtimeStream, clientStream, context, _protocol, cts.Token).ConfigureAwait(false);
//        if (!tryConnect.Success)
//        {
//            return;
//        }
//        var (dispatcher, arguments) = tryConnect.Result;
//        var executionContext = arguments.ExecutionContext;
//        var definition = arguments.ProjectionDefinition;
//        
//        Log.ReceivedProjectionArguments(_logger, definition.Scope, definition.Projection);
//        _logger.SettingExecutionContext(executionContext);
//        _executionContextManager.CurrentFor(executionContext);
//
//        
//        var projection = new Projection(dispatcher, definition, arguments.Alias, arguments.HasAlias);
//        var registration = await _projections.Register(projection, cts.Token);
//
//        if (!registration.Success)
//        {
//            Log.ErrorWhileRegisteringProjection(_logger, definition.Scope, definition.Projection, registration.Exception);
//            await dispatcher.Reject(new ProjectionRegistrationResponse
//            {
//                Failure = new Failure(
//                    ProjectionFailures.FailedToRegisterProjection,
//                    $"Failed to register Projection {projection.Definition.Projection}. {registration.Exception.Message}")
//            }, cts.Token).ConfigureAwait(false);
//            return;
//        }
//
//        using var processor = registration.Result;
//        var reverseCall = dispatcher.Accept(new ProjectionRegistrationResponse(), cts.Token);
//        var processing = processor.Start();
//
//        var tasks = new TaskGroup(reverseCall, processing);
//        
//        tasks.OnFirstTaskFailure += (_, ex) => Log.ErrorWhileRunningProjection(_logger, projection.Definition.Scope, projection.Definition.Projection, ex);
//        tasks.OnAllTasksCompleted += () => Log.ProjectionDisconnected(_logger, projection.Definition.Scope, projection.Definition.Projection);
//        
//        await tasks.WaitForAllCancellingOnFirst(cts).ConfigureAwait(false);
//    }
//}
