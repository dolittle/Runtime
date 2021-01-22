// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting.Stages;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Logging.Microsoft;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Extensions for building <see cref="LoggingSettings"/>.
    /// </summary>
    public static class LoggingBootBuilderExtensions
    {
        /// <summary>
        /// Set the log message writer creators to use.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="creators">The instances of <see cref="ILogMessageWriterCreator"/> to use.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder UseLogMessageWriterCreators(this IBootBuilder bootBuilder, params ILogMessageWriterCreator[] creators)
        {
            bootBuilder.Set<LoggingSettings>(_ => _.LogMessageWriterCreators, creators);
            return bootBuilder;
        }

        /// <summary>
        /// Set <see cref="ILoggerFactory"/> to use.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/> to use.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder UseLoggerFactory(this IBootBuilder bootBuilder, ILoggerFactory loggerFactory)
        {
            bootBuilder.Set<LoggingSettings>(_ => _.LogMessageWriterCreators, new[] { new LogMessageWriterCreator(loggerFactory) });
            return bootBuilder;
        }

        /// <summary>
        /// Disables logging.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder NoLogging(this IBootBuilder bootBuilder)
        {
            bootBuilder.Set<LoggingSettings>(_ => _.DisableLogging, true);
            return bootBuilder;
        }
    }
}
