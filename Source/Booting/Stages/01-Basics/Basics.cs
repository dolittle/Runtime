// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents the <see cref="BootStage.Basics"/> stage of booting.
    /// </summary>
    public class Basics : ICanPerformBootStage<BasicsSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.Basics;

        /// <inheritdoc/>
        public void Perform(BasicsSettings settings, IBootStageBuilder builder)
        {
            // get this isnt set at first to get the real dead entryassembly for something
            var entryAssembly = settings.EntryAssembly ?? Assembly.GetEntryAssembly();
            var environment = settings.Environment ?? Environment.Production;

            // add theses into the aggregatedAssociations in BootStages
            builder.Associate(WellKnownAssociations.EntryAssembly, entryAssembly);
            builder.Associate(WellKnownAssociations.Environment, environment);
            // bind Environment type to the one given from settings
            builder.Bindings.Bind<Environment>().To(environment);
        }
    }
}
