// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Processing.Management.StreamProcessors;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;
using Artifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="EventHandlersBase"/>.
/// </summary>
public class EventHandlersService : EventHandlersBase
{
    readonly IEventHandlers _eventHandlers;
    readonly IExceptionToFailureConverter _exceptionToFailureConverter;
    readonly IConvertStreamProcessorStatuses _streamProcessorStatusConverter;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
    /// </summary>
    /// <param name="eventHandlers">The <see cref="IEventHandlers"/> to use to perform operations on Event Handlers.</param>
    /// <param name="exceptionToFailureConverter">The <see cref="IExceptionToFailureConverter"/> to use to convert exceptions to failures.</param>
    /// <param name="streamProcessorStatusConverter">The <see cref="IConvertStreamProcessorStatuses"/> to use to convert stream processor states.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public EventHandlersService(
        IEventHandlers eventHandlers,
        IExceptionToFailureConverter exceptionToFailureConverter,
        IConvertStreamProcessorStatuses streamProcessorStatusConverter,
        ILogger logger)
    {
        _eventHandlers = eventHandlers;
        _exceptionToFailureConverter = exceptionToFailureConverter;
        _streamProcessorStatusConverter = streamProcessorStatusConverter;
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task<ReprocessEventsFromResponse> ReprocessEventsFrom(ReprocessEventsFromRequest request, ServerCallContext context)
    {
        var response = new ReprocessEventsFromResponse();
        var eventHandler = new EventHandlerId(request.ScopeId.ToGuid(), request.EventHandlerId.ToGuid());
        TenantId tenant = request.TenantId.ToGuid(); 
        _logger.ReprocessEventsFrom(eventHandler, tenant, request.StreamPosition);
        var reprocessing = await _eventHandlers.ReprocessEventsFrom(eventHandler, tenant, request.StreamPosition).ConfigureAwait(false);
        if (!reprocessing.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(reprocessing.Exception);
        }
            
        return response;
    }

    /// <inheritdoc />
    public override async Task<ReprocessAllEventsResponse> ReprocessAllEvents(ReprocessAllEventsRequest request, ServerCallContext context)
    {
        var response = new ReprocessAllEventsResponse();
        var eventHandler = new EventHandlerId(request.ScopeId.ToGuid(), request.EventHandlerId.ToGuid());
        _logger.ReprocessAllEvents(eventHandler);
        var reprocessing = await _eventHandlers.ReprocessAllEvents(eventHandler).ConfigureAwait(false);
        if (!reprocessing.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(reprocessing.Exception);
        }

        return response;
    }

    /// <inheritdoc />
    public override Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        _logger.GetAll();
        var eventHandlerInfos = _eventHandlers.All;

        foreach (var info in eventHandlerInfos)
        {
            var status = new EventHandlerStatus
            {   
                Alias = info.Alias,
                Partitioned = info.Partitioned,
                ScopeId = info.Id.Scope.ToProtobuf(),
                EventHandlerId = info.Id.EventHandler.ToProtobuf()
            };
            status.EventTypes.AddRange(CreateEventTypes(info));
                
            status.Tenants.AddRange(CreateScopedStreamProcessorStatus(info, request.TenantId?.ToGuid()));
                
            response.EventHandlers.Add(status);
        }

        return Task.FromResult(response);
    }

    IEnumerable<Artifact> CreateEventTypes(EventHandlerInfo info)
        => info.EventTypes.Select(_ => new Artifact()
        {
            Id = _.ToProtobuf(),
            Generation = ArtifactGeneration.First,
        });
        
    IEnumerable<TenantScopedStreamProcessorStatus> CreateScopedStreamProcessorStatus(EventHandlerInfo info, TenantId tenant = null)
    {
        var state = _eventHandlers.CurrentStateFor(info.Id);
        if (!state.Success)
        {
            throw state.Exception;
        }

        return tenant == null
            ? _streamProcessorStatusConverter.Convert(state.Result)
            : _streamProcessorStatusConverter.ConvertForTenant(state.Result, tenant);
    }
}
