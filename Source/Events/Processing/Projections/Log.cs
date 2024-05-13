// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Information,
        "Client is trying to connect to runtime Projections. Migrate client to V23 or newer.")]
    internal static partial void ConnectingUnsupportedProjections(this ILogger logger);
}
