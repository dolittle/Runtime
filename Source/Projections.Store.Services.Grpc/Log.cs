// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc;

/// <summary>
/// The log messages for <see cref="Grpc"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Sending ProjectionStore GetOne result for Key {Key} of Projection {Projection} in Scope {Scope} with State Type {Type}")]
    internal static partial void SendingGetOneResult(ILogger logger, string key, Uuid projection, Uuid scope, ProjectionCurrentStateType type);
    
    [LoggerMessage(0, LogLevel.Trace, "Sending ProjectionStore GetOne failed for Key {Key} of Projection {Projection} in Scope {Scope}")]
    internal static partial void SendingGetOneFailed(ILogger logger, string key, Uuid projection, Uuid scope, Exception exception);
    
    [LoggerMessage(0, LogLevel.Trace, "Sending ProjectionStore GetAll result for Projection {Projection} in Scope {Scope} with {Count} states")]
    internal static partial void SendingGetAllResult(ILogger logger, Uuid projection, Uuid scope, int count);
    
    [LoggerMessage(0, LogLevel.Trace, "Sending ProjectionStore GetAll failed for Projection {Projection} in Scope {Scope}")]
    internal static partial void SendingGetAllFailed(ILogger logger, Uuid projection, Uuid scope, Exception exception);
    
    [LoggerMessage(0, LogLevel.Trace, "Sending ProjectionStore GetAllInBatches result for Projection {Projection} in Scope {Scope} with {Count} states")]
    internal static partial void SendingGetAllInBatchesResult(ILogger logger, Uuid projection, Uuid scope, int count);
    
    [LoggerMessage(0, LogLevel.Trace, "Sending ProjectionStore GetAllInBatches failed for Projection {Projection} in Scope {Scope}")]
    internal static partial void SendingGetAllInBatchesFailed(ILogger logger, Uuid projection, Uuid scope, Exception exception);
}
