// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.Projections;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the implementation of <see cref="ProjectionsBase"/>.
/// </summary>
[PrivateService]
public class ProjectionsService : ProjectionsBase
{
    readonly IInitiateReverseCallServices _reverseCallServices;
    readonly IProjectionsProtocol _protocol;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly IProjections _projections;
    readonly IHostApplicationLifetime _hostApplicationLifetime;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsService"/> class.
    /// </summary>
    /// <param name="reverseCallServices">The initiator to use to start reverse call protocols.</param>
    /// <param name="protocol">The projections protocol to use to parse and create messages.</param>
    /// <param name="executionContextCreator">The execution context creator to use to validate execution contexts on requests.</param>
    /// <param name="projections">The <see cref="IProjections"/> to use to register projections.</param>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime"/> to use to stop ongoing reverse calls when the application is shutting down.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public ProjectionsService(
        IInitiateReverseCallServices reverseCallServices,
        IProjectionsProtocol protocol,
        ICreateExecutionContexts executionContextCreator,
        IProjections projections,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger logger)
    {
        _reverseCallServices = reverseCallServices;
        _protocol = protocol;
        _executionContextCreator = executionContextCreator;
        _projections = projections;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task Connect(
        IAsyncStreamReader<ProjectionClientToRuntimeMessage> runtimeStream,
        IServerStreamWriter<ProjectionRuntimeToClientMessage> clientStream,
        ServerCallContext context)
    {
        // TODO: It seems like things are not properly unregistered on exceptions?
        // TODO: I tested this out and while making the DI container work, it kept failing and telling me that the projection was already registered on the second attempt.

        Log.ConnectingProjections(_logger);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        var tryConnect = await _reverseCallServices.Connect(runtimeStream, clientStream, context, _protocol, cts.Token).ConfigureAwait(false);
        if (!tryConnect.Success)
        {
            return;
        }
        var (dispatcher, arguments) = tryConnect.Result;
        var executionContext = arguments.ExecutionContext;
        var definition = arguments.ProjectionDefinition;

        var tryCreateExecutionContext = _executionContextCreator.TryCreateUsing(arguments.ExecutionContext);
        if (!tryCreateExecutionContext.Success)
        {
            await dispatcher.Reject(
                _protocol.CreateFailedConnectResponse($"Failed to register Projection because the execution context is invalid: ${tryCreateExecutionContext.Exception.Message}"),
                cts.Token).ConfigureAwait(false);
            return;
        }
        
        Log.ReceivedProjectionArguments(_logger, definition.Scope, definition.Projection);

        
        var projection = new Projection(dispatcher, definition, arguments.Alias, arguments.HasAlias);
        var registration = await _projections.Register(projection, executionContext, cts.Token);

        if (!registration.Success)
        {
            Log.ErrorWhileRegisteringProjection(_logger, definition.Scope, definition.Projection, registration.Exception);
            await dispatcher.Reject(new ProjectionRegistrationResponse
            {
                Failure = new Failure(
                    ProjectionFailures.FailedToRegisterProjection,
                    $"Failed to register Projection {projection.Definition.Projection}. {registration.Exception.Message}")
            }, cts.Token).ConfigureAwait(false);
            return;
        }

        using var processor = registration.Result;
        var reverseCall = dispatcher.Accept(new ProjectionRegistrationResponse(), cts.Token);
        var processing = processor.Start();

        var tasks = new TaskGroup(reverseCall, processing);
        
        tasks.OnFirstTaskFailure += (_, ex) => Log.ErrorWhileRunningProjection(_logger, projection.Definition.Scope, projection.Definition.Projection, ex);
        tasks.OnAllTasksCompleted += () => Log.ProjectionDisconnected(_logger, projection.Definition.Scope, projection.Definition.Projection);
        
        await tasks.WaitForAllCancellingOnFirst(cts).ConfigureAwait(false);
    }
}
