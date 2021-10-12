// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Projections.Store.Services
{
    static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Guid, string, Exception> _gettingOneProjection = LoggerMessage
            .Define<Guid, Guid, string>(
                LogLevel.Debug,
                new EventId(684554295, nameof(GettingOneProjection)),
                "Getting state for projection {Projection} in scope {Scope} with key {Key}");

        static readonly Action<ILogger, Guid, Guid, Exception> _gettingAllProjections = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(684554295, nameof(GettingOneProjection)),
                "Getting all states for projection {Projection} in scope {Scope}");

        internal static void GettingOneProjection(this ILogger logger, ProjectionId projection, ScopeId scope, ProjectionKey key)
            => _gettingOneProjection(logger, projection, scope, key, null);


        internal static void GettingAllProjections(this ILogger logger, ProjectionId projection, ScopeId scope)
            => _gettingAllProjections(logger, projection, scope, null);
    }
}