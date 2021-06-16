// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents the settings for <see cref="BootStage.BootProcedures"/> stage.
    /// </summary>
    public class BootProceduresSettings : IRepresentSettingsForBootStage
    {
        /// <summary>
        /// Gets a value indicating whether or not it boot procedures are enabled.
        /// </summary>
        public bool Enabled { get; internal set; } = true;
    }
}