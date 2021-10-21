// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        static readonly Action<ILogger, ulong, Guid, Guid, Guid, Exception> _reprocessEventsFrom = LoggerMessage
            .Define<ulong, Guid, Guid, Guid>(
                LogLevel.Information,
                new EventId(45433, nameof(ReprocessEventsFrom)),
                "Reprocessing events from position {Position} for event handler {EventHandler} and tenant {Tenant} in scope {Scope}");
        
        static readonly Action<ILogger, Guid, Guid, Exception> _reprocessAllEvents = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Information,
                new EventId(4233, nameof(ReprocessAllEvents)),
                "Reprocessing all events for event handler {EventHandler} for all tenants in scope {Scope}");
        
        static readonly Action<ILogger, Exception> _getAllEventHandlers = LoggerMessage
            .Define(
                LogLevel.Information,
                new EventId(414912821, nameof(GetAll)),
                "Getting all running event handlers");

        internal static void ReprocessEventsFrom(this ILogger logger, EventHandlerId eventHandler, TenantId tenant, StreamPosition position)
            => _reprocessEventsFrom(logger, position, eventHandler.EventHandler, tenant, eventHandler.Scope, null);

        internal static void ReprocessAllEvents(this ILogger logger, EventHandlerId eventHandler)
            => _reprocessAllEvents(logger, eventHandler.EventHandler, eventHandler.Scope, null);
        internal static void GetAll(this ILogger logger)
            => _getAllEventHandlers(logger, null);
    }
}
