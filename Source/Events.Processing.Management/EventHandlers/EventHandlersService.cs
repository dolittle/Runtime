// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers
{
    public class EventHandlersService : EventHandlersBase
    {
        readonly IEventHandlers _eventHandlers;
        readonly IExceptionToFailureConverter _exceptionToFailureConverter;
        readonly ILogger _logger;
        
        public EventHandlersService(
            IEventHandlers eventHandlers,
            IExceptionToFailureConverter exceptionToFailureConverter,
            ILogger logger)
        {
            _eventHandlers = eventHandlers;
            _exceptionToFailureConverter = exceptionToFailureConverter;
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
    }
}