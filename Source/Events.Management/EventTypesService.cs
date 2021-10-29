// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Management.Contracts.EventTypes;
using GetAllRequest = Dolittle.Runtime.Events.Management.Contracts.GetAllRequest;
using GetAllResponse = Dolittle.Runtime.Events.Management.Contracts.GetAllResponse;

namespace Dolittle.Runtime.Events.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="EventTypesBase"/>.
    /// </summary>
    public class EventTypesService : EventTypesBase
    {
        readonly IEventTypes _eventTypes;
        readonly ILogger _logger;
        
        public EventTypesService(IEventTypes eventTypes, ILogger logger)
        {
            _eventTypes = eventTypes;
            _logger = logger;
        }

        /// <inheritdoc />
        public override Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            try
            {
                _logger.GetAll();
                var response = new GetAllResponse();
                
                response.EventTypes.AddRange(_eventTypes.All.Select(ToProtobuf));
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Failure(ex);
                return Task.FromResult(new GetAllResponse { Failure = ex.ToProtobuf() });
            }
        }

        static Events.Management.Contracts.EventType ToProtobuf(EventType eventType)
            => new()
            {
                Alias = eventType.Alias,
                EventType_ = eventType.Identifier.ToProtobuf()
            };
    }
}
