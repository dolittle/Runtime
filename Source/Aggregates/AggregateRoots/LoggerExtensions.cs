// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Aggregates.AggregateRoots
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        // static readonly Action<ILogger, Exception> _receivedEventHandler = LoggerMessage
        //     .Define<>(
        //         LogLevel.Trace,
        //         new EventId(, nameof(ReceivedEventHandler)),
        //         "Received arguments for event handler {EventHandler} with source stream: {SourceStream} scope: {Scope} filtered on event types: {Types}\nPartitioned: {Partitioned}");
        //
        //
        // internal static void ReceivedEventHandler(this ILogger logger)
        //     => _receivedEventHandler(logger, null);
    }
}
