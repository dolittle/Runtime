// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Metrics;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Starting metrics server on port {Port} on path '{Path}'")]
    internal static partial void StartingMetricsServer(ILogger logger, int port, string path);
}
