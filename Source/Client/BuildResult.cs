// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Represents a client <see cref="BuildResult"/>-
/// </summary>
/// <param name="Type">The build result <see cref="BuildResultType"/>.</param>
/// <param name="Message">The build result message.</param>
public record BuildResult(BuildResultType Type, string Message)
{
    /// <summary>
    /// Gets the <see cref="Microsoft.Extensions.Logging.LogLevel"/>.
    /// </summary>
    public LogLevel LogLevel => Type switch
    {
        BuildResultType.Information => LogLevel.Debug,
        BuildResultType.Failure => LogLevel.Warning,
        BuildResultType.Error => LogLevel.Error,
        _ => LogLevel.Debug
    };

    public void Log(ILogger logger, HeadId head)
    {
        if (logger.IsEnabled(LogLevel))
        {
            logger.Log(LogLevel, "{Type} Build Result for Head {Head}: {Message}", Type, head, Message);
        }
    }
}
