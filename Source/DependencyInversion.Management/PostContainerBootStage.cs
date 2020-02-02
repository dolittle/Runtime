// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Booting;
using Dolittle.DependencyInversion;
using Dolittle.DependencyInversion.Booting.Stages;

namespace Dolittle.Runtime.DependencyInversion.Management
{
    /// <summary>
    /// System that gets run before the <see cref="BootStage.Container"/> boot stage.
    /// </summary>
    public class PostContainerBootStage : ICanRunAfterBootStage<ContainerSettings>
    {
        /// <summary>
        /// <see cref="BindingCollection"/> with all bindings.
        /// </summary>
        internal static BindingCollection AllBindings;

        /// <inheritdoc/>
        public BootStage BootStage => BootStage.Container;

        /// <inheritdoc/>
        public void Perform(ContainerSettings settings, IBootStageBuilder builder)
        {
            var bindings = builder.GetAssociation(WellKnownAssociations.Bindings) as IBindingCollection;
            AllBindings = new BindingCollection(bindings.ToArray());
        }
    }
}