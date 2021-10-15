// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers
{
    public class EventHandlersService : EventHandlersBase
    {
        readonly ILogger _logger;

        public EventHandlersService(ILogger logger)
        {
            _logger = logger;
        }

        public override Task<ReprocessEventsFromResponse> ReprocessEventsFrom(ReprocessEventsFromRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ReprocessEventsFrom called");
            return Task.FromResult(new ReprocessEventsFromResponse());
        }

        public override Task<ReprocessAllEventsResponse> ReprocessAllEvents(ReprocessAllEventsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ReprocessAllEvents called");
            return Task.FromResult(new ReprocessAllEventsResponse());
        }
    }
}