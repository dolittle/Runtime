// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;
using Failure = Dolittle.Protobuf.Contracts.Failure;

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
            
            var (dispatcher, arguments) = connectResult.Result;
            if (arguments.Scope != ScopeId.Default && _configuration.Value.Fast)
            {
                var exception = new FastEventHandlerFilterNotYetSupported(arguments);
                _logger.FastScopedEventHandlerNotSupported(exception);
                await dispatcher.Reject(new EventHandlerRegistrationResponse
                {
                    Failure = new Failure
                    {
                        Id = Failures.Unknown.ToProtobuf(),
                        Reason = exception.Message
                    }
                }, context.CancellationToken).ConfigureAwait(false);
                return;
            }
            using var eventHandler = _configuration.Value.Fast
                ? _eventHandlerFactory.CreateFast(arguments, _configuration.Value.ImplicitFilter, dispatcher, context.CancellationToken)
                : _eventHandlerFactory.Create(arguments, dispatcher, context.CancellationToken); 

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

/// <summary>
/// Exception that gets thrown when attempting to register a "fast" scoped event handler. This will be removed whenever we start supporting it. 
/// </summary>
public class FastEventHandlerFilterNotYetSupported : Exception
{
    public FastEventHandlerFilterNotYetSupported(EventHandlerRegistrationArguments arguments)
        : base($"Failed to register event handler {arguments.EventHandler} in scope {arguments.Scope} The fast event handler type is not yet supported for scoped event handlers. Either turn off fast event handlers by setting the environment variable `DOLITTLE__RUNTIME__PROCESSING__EVENTHANDLERS__FAST` to `false` or wait until a new version that supports this is released ")
    {
    }
}
