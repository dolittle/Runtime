// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Booting.Stages;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "<********* BOOTSTAGE : Logging *********>")]
    internal static partial void BootStageLogging(ILogger logger);
}
