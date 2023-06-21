// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.EventHandlers.Actors;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;
using Proto;
using ReverseCallDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;


namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IEventHandlerFactory"/>.
/// </summary>
public class EventHandlerFactory : IEventHandlerFactory
{
    readonly CreateStreamProcessorActorProps _createStreamProcessorActorProps;
    readonly IStreamDefinitions _streamDefinitions;
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;
    readonly ITenants _tenants;
    readonly ActorSystem _actorSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerFactory"/> class.
    /// </summary>
    /// <param name="streamDefinitions">The <see cref="IStreamDefinitions"/>.</param>
    /// <param name="metrics">The collector to use for metrics.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    /// <param name="tenants">The <see cref="ITenants"/>.</param>
    /// <param name="actorSystem"></param>
    /// <param name="createStreamProcessorActorProps">Create Actor props for stream processor actor</param>
    public EventHandlerFactory(
        IStreamDefinitions streamDefinitions,
        IMetricsCollector metrics,
        ILoggerFactory loggerFactory,
        ITenants tenants,
        ActorSystem actorSystem, CreateStreamProcessorActorProps createStreamProcessorActorProps, IApplicationLifecycleHooks lifecycleHooks)
    {
        _streamDefinitions = streamDefinitions;
        _metrics = metrics;
        _loggerFactory = loggerFactory;
        _tenants = tenants;
        _actorSystem = actorSystem;
        _createStreamProcessorActorProps = createStreamProcessorActorProps;
    }

    /// <inheritdoc />
    public IEventHandler Create(EventHandlerRegistrationArguments arguments, ReverseCallDispatcher dispatcher, CancellationToken cancellationToken)
    {
        EventProcessor Converter(TenantId _) => new(arguments.Scope, arguments.EventHandler, dispatcher, _loggerFactory.CreateLogger<EventProcessor>());
        return new ActorEventHandler(_streamDefinitions,
            arguments,
            Converter,
            cancellation => dispatcher.Accept(new EventHandlerRegistrationResponse(), cancellation),
            (failure, cancellation) => dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure.ToProtobuf() }, cancellation),
            _metrics,
            _loggerFactory.CreateLogger<EventHandler>(),
            arguments.ExecutionContext,
            _actorSystem,
            _tenants,
            _createStreamProcessorActorProps,
            cancellationToken
        );
    }

}
