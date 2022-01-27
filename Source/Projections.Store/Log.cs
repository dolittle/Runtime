// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Projections.Store;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting state for projection {Projection} in scope {Scope} with key {Key}")]
    internal static partial void GettingOneProjection(ILogger logger, ProjectionId projection, ScopeId scope, ProjectionKey key);

    [LoggerMessage(0, LogLevel.Debug, "Getting all states for projection {Projection} in scope {Scope}")]
    internal static partial void GettingAllProjections(ILogger logger, ProjectionId projection, ScopeId scope);

    [LoggerMessage(0, LogLevel.Warning, "Error getting projection")]
    internal static partial void ErrorGettingOneProjection(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Warning, "Error getting all projections")]
    internal static partial void ErrorGettingAllProjections(ILogger logger, Exception ex);


    [LoggerMessage(0, LogLevel.Debug, "Replacing state for projection {Projection} in scope {Scope} with key {Key}")]
    internal static partial void ReplacingProjectionState(ILogger logger, ProjectionId projection, ScopeId scope, ProjectionKey key);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to replace state for projection {Projection} in scope {Scope} with key {Key} in copy store. Will skip next copy stores an projection store to avoid data corruption")]
    internal static partial void FailedToReplaceProjectionStateInCopyStore(ILogger logger, ProjectionId projection, ScopeId scope, ProjectionKey key);

    [LoggerMessage(0, LogLevel.Debug, "Removing state for projection {Projection} in scope {Scope} with key {Key}")]
    internal static partial void RemovingProjectionState(ILogger logger, ProjectionId projection, ScopeId scope, ProjectionKey key);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to remove state for projection {Projection} in scope {Scope} with key {Key} in copy store. Will skip next copy stores an projection store to avoid data corruption")]
    internal static partial void FailedToRemoveProjectionStateInCopyStore(ILogger logger, ProjectionId projection, ScopeId scope, ProjectionKey key);

    [LoggerMessage(0, LogLevel.Debug, "Dropping projection {Projection} in scope {Scope}")]
    internal static partial void DroppingProjection(ILogger logger, ProjectionId projection, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to drop projection {Projection} in scope {Scope} in copy store. Will skip next copy stores an projection store to avoid data corruption")]
    internal static partial void FailedToDropProjectionInCopyStore(ILogger logger, ProjectionId projection, ScopeId scope);
}
