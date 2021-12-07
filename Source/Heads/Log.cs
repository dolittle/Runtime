// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Heads;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Disconnecting head '{HeadId}'")]
    internal static partial void DisconnectingHead(ILogger logger, HeadId headId);
}
