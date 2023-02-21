// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client.Management;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class LoggerExtensions
{
    [LoggerMessage(0, LogLevel.Information, "Getting client build results")]
    internal static partial void GettingClientBuildResults(this ILogger logger);
}
