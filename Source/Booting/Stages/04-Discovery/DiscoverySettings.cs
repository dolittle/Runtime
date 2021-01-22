// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Assemblies;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents the settings for <see cref="BootStage.InitialSystem"/> stage.
    /// </summary>
    public class DiscoverySettings : IRepresentSettingsForBootStage
    {
        /// <summary>
        /// Gets the collection of assembly names that can be included in discovery, based on starting with name.
        /// </summary>
        public IEnumerable<string> IncludeAssembliesStartWith { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ICanProvideAssemblies">assembly provider</see> to use.
        /// </summary>
        public ICanProvideAssemblies AssemblyProvider { get; internal set; }
    }
}