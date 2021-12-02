// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Execution;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Setting initial execution context")]
    internal static partial void SettingInitialExecutionContext(ILogger logger);
    [LoggerMessage(0, LogLevel.Trace, "Setting execution context ({NewContext}) - from: ({FilePath}, {LineNumber}, {Member})")]
    internal static partial void SettingExecutionContext(ILogger logger, ExecutionContext newContext, string filePath, int lineNumber, string member);
}
