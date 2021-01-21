// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Logging.Internal;

namespace Dolittle.Runtime.Specs.Logging.for_LoggerManager.given
{
    public static class LoggerExtensions
    {
        public static ILogMessageWriter[] GetLogMessageWriters(this ILogger logger)
        {
            var internalLogger = logger as InternalLogger;
            return internalLogger?.LogMessageWriters;
        }
    }
}