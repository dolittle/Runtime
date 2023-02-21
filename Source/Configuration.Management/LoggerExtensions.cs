// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Configuration.Management;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class LoggerExtensions
{
    [LoggerMessage(0, LogLevel.Information, "Getting configuration as YAML")]
    internal static partial void GettingConfigurationYaml(this ILogger logger);
}
