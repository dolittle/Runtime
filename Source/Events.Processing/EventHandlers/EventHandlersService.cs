// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents the implementation of <see cref="EventHandlersBase"/>.
/// </summary>
public class EventHandlersService : EventHandlersBase
{
    readonly IExecutionContextManager _executionContextManager;
    readonly IInitiateReverseCallServices _reverseCallServices;
    readonly IEventHandlersProtocol _eventHandlersProtocol;
    readonly IEventHandlers _eventHandlers;
    readonly ILogger _logger;
    readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
    /// </summary>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
    /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
    /// <param name="reverseCallServices">The <see cref="IInitiateReverseCallServices" />.</param>
    /// <param name="eventHandlersProtocol">The <see cref="IEventHandlersProtocol" />.</param>
    /// <param name="eventHandlers">The <see cref="IEventHandlers" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public EventHandlersService(
        IHostApplicationLifetime hostApplicationLifetime,
        IExecutionContextManager executionContextManager,
        IInitiateReverseCallServices reverseCallServices,
        IEventHandlersProtocol eventHandlersProtocol,
        IEventHandlers eventHandlers,
        ILoggerFactory loggerFactory)
    {
        _executionContextManager = executionContextManager;
        _reverseCallServices = reverseCallServices;
        _eventHandlersProtocol = eventHandlersProtocol;
        _eventHandlers = eventHandlers;
        _logger = loggerFactory.CreateLogger<EventHandlersService>();
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    /// <inheritdoc/>
    public override async Task Connect(
        IAsyncStreamReader<EventHandlerClientToRuntimeMessage> runtimeStream,
        IServerStreamWriter<EventHandlerRuntimeToClientMessage> clientStream,
        ServerCallContext context)
    {
        Log.ConnectingEventHandler(_logger);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        try
        {
            var connectResult = await _reverseCallServices.Connect(
                runtimeStream,
                clientStream,
                context,
                _eventHandlersProtocol,
                cts.Token).ConfigureAwait(false);
            if (!connectResult.Success)
            {
                return;
            }

            var (dispatcher, arguments) = connectResult.Result;
            _logger.SettingExecutionContext(arguments.ExecutionContext);
            _executionContextManager.CurrentFor(arguments.ExecutionContext);
            await _eventHandlers.RegisterAndStart(dispatcher, arguments, cts.Token).ConfigureAwait(false);
        }
        finally
        {
            cts.Cancel();
        }
    }
}
