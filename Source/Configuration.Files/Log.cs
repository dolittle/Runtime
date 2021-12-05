// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Configuration.Files;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Parsing '{Filename}' into '{ConfigurationObjectName} - {ConfigurationObjectType}'")]
    internal static partial void ParsingFileIntoConfiguration(ILogger logger, string fileName, string configurationObjectName, string configurationObjectType);
}
