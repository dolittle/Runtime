// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Logging;

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
        /// Gets the log message writer creators to use when creating instances of <see cref="ILogger"/>.
        /// </summary>
        public IEnumerable<ILogMessageWriterCreator> LogMessageWriterCreators { get; internal set; }
    }
}
