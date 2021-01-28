// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents the settings for <see cref="BootStage.Logging"/> stage.
    /// </summary>
    public class LoggingSettings : IRepresentSettingsForBootStage
    {
        /// <summary>
        /// Gets a value indicating whether logging should be disabled.
        /// If true all instance of <see cref="ILogger"/> will not write any logs.
        /// </summary>
        public bool DisableLogging { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether logging should be disabled.
        /// If true all instance of <see cref="ILogger"/> will not write any logs.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; internal set; } = Microsoft.Extensions.Logging.LoggerFactory.Create(logging =>
        {
            logging.ClearProviders();
        });
    }
}
