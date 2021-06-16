// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Booting;

namespace Dolittle.Runtime.DependencyInversion.Booting.Stages
{
    /// <summary>
    /// Represents the settings for <see cref="BootStage.Container"/> stage.
    /// </summary>
    public class ContainerSettings : IRepresentSettingsForBootStage
    {
        /// <summary>
        /// Gets the <see cref="IContainer"/> type.
        /// </summary>
        public Type ContainerType { get; internal set; }
    }
}