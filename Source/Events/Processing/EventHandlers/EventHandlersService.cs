// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents the implementation of <see cref="EventHandlersBase"/>.
/// </summary>
[PrivateService]
public class EventHandlersService : EventHandlersBase
{
    readonly IInitiateReverseCallServices _reverseCallServices;
    readonly IEventHandlersProtocol _eventHandlersProtocol;
    readonly IEventHandlers _eventHandlers;
    readonly IEventHandlerFactory _eventHandlerFactory;
    readonly IOptions<EventHandlersConfiguration> _configuration;
    readonly ILogger _logger;
    readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
    /// </summary>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
    /// <param name="reverseCallServices">The <see cref="IInitiateReverseCallServices" />.</param>
    /// <param name="eventHandlersProtocol">The <see cref="IEventHandlersProtocol" />.</param>
    /// <param name="eventHandlers">The <see cref="IEventHandlers" />.</param>
    /// <param name="eventHandlerFactory">The <see cref="IEventHandlerFactory"/>.</param>
    /// <param name="configuration">The <see cref="EventHandlersConfiguration"/></param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public EventHandlersService(
        IHostApplicationLifetime hostApplicationLifetime,
        IInitiateReverseCallServices reverseCallServices,
        IEventHandlersProtocol eventHandlersProtocol,
        IEventHandlers eventHandlers,
        IEventHandlerFactory eventHandlerFactory,
        IOptions<EventHandlersConfiguration> configuration,
        ILoggerFactory loggerFactory)
    {
        _reverseCallServices = reverseCallServices;
        _eventHandlersProtocol = eventHandlersProtocol;
        _eventHandlers = eventHandlers;
        _eventHandlerFactory = eventHandlerFactory;
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<EventHandlersService>();
        
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    /// <inheritdoc/>
    public override async Task Connect(
        IAsyncStreamReader<EventHandlerClientToRuntimeMessage> runtimeStream,
        IServerStreamWriter<EventHandlerRuntimeToClientMessage> clientStream,
        ServerCallContext context)
    {
        _logger.ConnectingEventHandler();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        try
        {
            var connectResult = await _reverseCallServices.Connect(runtimeStream, clientStream, context, _eventHandlersProtocol, cts.Token).ConfigureAwait(false);
            if (!connectResult.Success)
            {
                return;
            }
            
            using var dispatcher = connectResult.Result.dispatcher;
            var arguments = connectResult.Result.arguments;
            using var eventHandler = _eventHandlerFactory.Create(arguments, dispatcher, context.CancellationToken);

            await _eventHandlers.RegisterAndStart(
                eventHandler,
                (failure, cancellation) => dispatcher.Reject(new EventHandlerRegistrationResponse{Failure = failure.ToProtobuf()}, cancellation),
                cts.Token).ConfigureAwait(false);
        }
        finally
        {
            cts.Cancel();
        }
    }
}
