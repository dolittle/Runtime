// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.HealthChecks;

/// <summary>
/// Log messages for <see cref="Dolittle.Runtime.Services.HealthChecks"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Adding gRPC endpoint health check for {Visibility}")]
    internal static partial void AddingHealthCheckFor(this ILogger logger, EndpointVisibility visibility);
}
