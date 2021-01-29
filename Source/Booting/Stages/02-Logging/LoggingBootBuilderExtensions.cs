/// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting.Stages;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Extensions for building <see cref="LoggingSettings"/>.
    /// </summary>
    public static class LoggingBootBuilderExtensions
    {
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

        public static IBootBuilder WithLoggingFactory(this IBootBuilder bootBuilder, ILoggerFactory loggerFactory)
        {
            bootBuilder.Set<LoggingSettings>(_ => _.LoggerFactory, loggerFactory);
            return bootBuilder;
        }
    }
}
