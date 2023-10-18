// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Processing.Management.StreamProcessors;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;
using Artifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="EventHandlersBase"/>.
/// </summary>
[ManagementService, ManagementWebService]
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
    public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        Log.GetAll(_logger);
        var response = new GetAllResponse();
        foreach (var eventHandlerInfo in _eventHandlers.All)
        {
            var result = await CreateStatusFromInfo(eventHandlerInfo, request.TenantId?.ToGuid());
            response.EventHandlers.Add(result);
        }
        
        return response;
    }

    /// <inheritdoc />
    public override async Task<GetOneResponse> GetOne(GetOneRequest request, ServerCallContext context)
    {
        var response = new GetOneResponse();

        var getIds = GetEventHandlerId(request.ScopeId, request.EventHandlerId);
        if (!getIds.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(getIds.Exception);
            return response;
        }

        var eventHandler = getIds.Result;
        
        Log.GetOne(_logger, eventHandler.EventHandler, eventHandler.Scope);

        var info = _eventHandlers.All.FirstOrDefault(_ => _.Id == eventHandler);
        if (info == default)
        {
            Log.EventHandlerNotRegistered(_logger, eventHandler.EventHandler, eventHandler.Scope);
            response.Failure = _exceptionToFailureConverter.ToFailure(new EventHandlerNotRegistered(eventHandler));
            return response;
        }

        response.EventHandlers = await CreateStatusFromInfo(info, request.TenantId?.ToGuid());
        return response;
    }
    
    
    /// <inheritdoc />
    public override async Task<ReprocessEventsFromResponse> ReprocessEventsFrom(ReprocessEventsFromRequest request, ServerCallContext context)
    {
        var response = new ReprocessEventsFromResponse();

        var getIds = GetEventHandlerId(request.ScopeId, request.EventHandlerId);
        if (!getIds.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(getIds.Exception);
            return response;
        }

        var eventHandler = getIds.Result;
        
        TenantId tenant = request.TenantId.ToGuid(); 
        Log.ReprocessEventsFrom(_logger, eventHandler.EventHandler, eventHandler.Scope, tenant, request.StreamPosition);

        var eventLogPosition = new EventLogSequenceNumber(request.StreamPosition);
        var processingPosition = new ProcessingPosition(StreamPosition.Start, eventLogPosition);

        var reprocessing = await _eventHandlers.ReprocessEventsFrom(eventHandler, tenant, processingPosition).ConfigureAwait(false);
        if (!reprocessing.Success)
        {
            Log.FailedDuringReprocessing(_logger, reprocessing.Exception);
            response.Failure = _exceptionToFailureConverter.ToFailure(reprocessing.Exception);
        }
            
        return response;
    }

    /// <inheritdoc />
    public override async Task<ReprocessAllEventsResponse> ReprocessAllEvents(ReprocessAllEventsRequest request, ServerCallContext context)
    {
        var response = new ReprocessAllEventsResponse();
        
        var getIds = GetEventHandlerId(request.ScopeId, request.EventHandlerId);
        if (!getIds.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(getIds.Exception);
            return response;
        }

        var eventHandler = getIds.Result;
        
        Log.ReprocessAllEvents(_logger, eventHandler.EventHandler, eventHandler.Scope);
        var reprocessing = await _eventHandlers.ReprocessAllEvents(eventHandler).ConfigureAwait(false);
        if (!reprocessing.Success)
        {
            Log.FailedDuringReprocessing(_logger, reprocessing.Exception);
            response.Failure = _exceptionToFailureConverter.ToFailure(reprocessing.Exception);
        }

        return response;
    }

    async Task<EventHandlerStatus> CreateStatusFromInfo(EventHandlerInfo info, TenantId tenant)
    {
        var status = new EventHandlerStatus
        {   
            Alias = info.Alias,
            Partitioned = info.Partitioned,
            ScopeId = info.Id.Scope.ToProtobuf(),
            EventHandlerId = info.Id.EventHandler.ToProtobuf()
        };
        status.EventTypes.AddRange(info.EventTypes.Select(CreateEventType));
        status.Tenants.AddRange(await CreateScopedStreamProcessorStatus(info, tenant));
        return status;
    }

    async Task<IEnumerable<TenantScopedStreamProcessorStatus>> CreateScopedStreamProcessorStatus(EventHandlerInfo info, TenantId tenant = null)
    {
        var state = await _eventHandlers.CurrentStateFor(info.Id);
        if (!state.Success)
        {
            throw state.Exception;
        }

        return tenant == null
            ? _streamProcessorStatusConverter.Convert(state.Result)
            : _streamProcessorStatusConverter.ConvertForTenant(state.Result, tenant);
    }

    static Artifact CreateEventType(ArtifactId id)
        => new()
        {
            Id = id.ToProtobuf(),
            Generation = ArtifactGeneration.First,
        };

    static Try<EventHandlerId> GetEventHandlerId(Uuid? scope, Uuid? eventHandler)
    {
        if (scope == default)
        {
            return Try<EventHandlerId>.Failed(new ArgumentNullException(nameof(scope), "Scope id is missing in request"));
        }
        if (eventHandler == default)
        {
            return Try<EventHandlerId>.Failed(new ArgumentNullException(nameof(eventHandler), "EventHandler id is missing in request"));
        }

        return Try<EventHandlerId>.Succeeded(new EventHandlerId(scope.ToGuid(), eventHandler.ToGuid()));
    }
}
