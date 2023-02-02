// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
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
    readonly IStreamProcessors _streamProcessors;
    readonly IValidateFilterForAllTenants _filterValidator;
    readonly Func<TenantId, IWriteEventsToStreams> _getEventsToStreamsWriter;   
    readonly IStreamDefinitions _streamDefinitions;
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerFactory"/> class.
    /// </summary>
    /// <param name="streamProcessors">The <see cref="IStreamProcessors"/>.</param>
    /// <param name="filterValidator">The <see cref="IValidateFilterForAllTenants"/>.</param>
    /// <param name="getEventsToStreamsWriter">The <see cref="Func{TResult}"/> for getting a tenant scoped <see cref="IWriteEventsToStreams"/>.</param>
    /// <param name="streamDefinitions">The <see cref="IStreamDefinitions"/>.</param>
    /// <param name="metrics">The collector to use for metrics.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    /// <param name="filterStreamProcessors">The <see cref="IFilterStreamProcessors"/>.</param>
    public EventHandlerFactory(
        IStreamProcessors streamProcessors,
        IValidateFilterForAllTenants filterValidator,
        Func<TenantId, IWriteEventsToStreams> getEventsToStreamsWriter,
        IStreamDefinitions streamDefinitions,
        IMetricsCollector metrics,
        ILoggerFactory loggerFactory)
    {
        _streamProcessors = streamProcessors;
        _filterValidator = filterValidator;
        _getEventsToStreamsWriter = getEventsToStreamsWriter;
        _streamDefinitions = streamDefinitions;
        _metrics = metrics;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IEventHandler Create(EventHandlerRegistrationArguments arguments, ReverseCallDispatcher dispatcher, CancellationToken cancellationToken)
        => new EventHandler(
            _streamProcessors,
            _filterValidator,
            _streamDefinitions,
            arguments,
            tenant => new TypeFilterWithEventSourcePartition(
                arguments.Scope,
                new TypeFilterWithEventSourcePartitionDefinition(StreamId.EventLog, arguments.EventHandler.Value, arguments.EventTypes ,arguments.Partitioned),
                _getEventsToStreamsWriter(tenant),
                _loggerFactory.CreateLogger<TypeFilterWithEventSourcePartition>()),
            _ => new EventProcessor(arguments.Scope, arguments.EventHandler, dispatcher, _loggerFactory.CreateLogger<EventProcessor>()),
            cancellation => dispatcher.Accept(new EventHandlerRegistrationResponse(), cancellation),
            (failure, cancellation) => dispatcher.Reject(new EventHandlerRegistrationResponse{Failure = failure.ToProtobuf()}, cancellation),
            _metrics,
            _loggerFactory.CreateLogger<EventHandler>(),
            arguments.ExecutionContext,
            cancellationToken
        );
}
