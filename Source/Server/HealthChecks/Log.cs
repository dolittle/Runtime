// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Server.HealthChecks;

/// <summary>
/// Log messages for <see cref="Dolittle.Runtime.Server.HealthChecks"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Adding health check implementation {HealthCheckType}")]
    internal static partial void AddingHealthCheckRegistrationFor(this ILogger logger, Type healthCheckType);
}
