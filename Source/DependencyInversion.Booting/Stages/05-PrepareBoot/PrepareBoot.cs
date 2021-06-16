// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;

namespace Dolittle.Runtime.DependencyInversion.Booting.Stages
{
    /// <summary>
    /// Represents the <see cref="BootStage.PrepareBoot"/> stage of booting.
    /// </summary>
    public class PrepareBoot : ICanPerformBootStage<NoSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.PrepareBoot;

        /// <inheritdoc/>
        public void Perform(NoSettings settings, IBootStageBuilder builder)
        {
            var bindings = builder.GetAssociation<IBindingCollection>(WellKnownAssociations.Bindings);
            var newBindingsNotifier = builder.GetAssociation<ICanNotifyForNewBindings>(WellKnownAssociations.NewBindingsNotificationHub);
            var bootContainer = new BootContainer(bindings, newBindingsNotifier);
            builder.UseContainer(bootContainer);
        }
    }
}