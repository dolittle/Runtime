// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

/// <summary>
/// Log messages for Projections Management.
/// </summary>
public static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Received Projections Management GetAll request.")]
    internal static partial void GetAll(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Information, "Received Projections Management GetOne request for projection {Projection} in scope {Scope}.")]
    internal static partial void GetOne(ILogger logger, ProjectionId projection, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Information, "Received Projections Management Replay request for projection {Projection} in scope {Scope}.")]
    internal static partial void Replay(ILogger logger, ProjectionId projection, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Warning, "Projection {Projection} in scope {Scope} is not registered.")]
    internal static partial void ProjectionNotRegistered(ILogger logger, ProjectionId projection, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Error, "Failed to replay projection {Projection} in scope {Scope}.")]
    internal static partial void FailedToReplayProjection(ILogger logger, ProjectionId projection, ScopeId scope, Exception exception);

    [LoggerMessage(0, LogLevel.Debug, "Creating status for projection {Projection} in scope {Scope} for tenant {Tenant}.")]
    internal static partial void CreatingProjectionStatusForTenant(ILogger logger, ProjectionId projection, ScopeId scope, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Debug, "Creating status for projection {Projection} in scope {Scope} for all tenants.")]
    internal static partial void CreatingProjectionStatusForAllTenants(ILogger logger, ProjectionId projection, ScopeId scope);
}
