// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Booting;
using Dolittle.Runtime.IO;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Scheduling;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.DependencyInversion.Booting.Stages
{
    /// <summary>
    /// Represents the <see cref="BootStage.PrepareBoot"/> stage of booting.
    /// </summary>
    public class Container : ICanPerformBootStage<ContainerSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.Container;

        /// <inheritdoc/>
        public void Perform(ContainerSettings settings, IBootStageBuilder builder)
        {
            IBindingCollection resultingBindings;
            var logger = new LoggerFactory().CreateLogger<Container>();
            var typeFinder = builder.GetAssociation(WellKnownAssociations.TypeFinder) as ITypeFinder;
            var scheduler = builder.GetAssociation(WellKnownAssociations.Scheduler) as IScheduler;

            var bindings = builder.GetAssociation(WellKnownAssociations.Bindings) as IBindingCollection;
            var assemblies = builder.GetAssociation(WellKnownAssociations.Assemblies) as IAssemblies;

            var fileSystem = new FileSystem();

            if (settings.ContainerType != null)
            {
                logger.LogTrace("Starting DependencyInversion with predefined container type '{containerType}'", settings.ContainerType.AssemblyQualifiedName);
                resultingBindings = Boot.Start(assemblies, typeFinder, scheduler, fileSystem, settings.ContainerType, bindings, builder.Container as BootContainer);
            }
            else
            {
                var bootResult = Boot.Start(assemblies, typeFinder, scheduler, fileSystem, bindings, builder.Container as BootContainer);
                resultingBindings = bootResult.Bindings;
                builder.UseContainer(bootResult.Container);
                logger.LogTrace("Using container of type '{containerType}'", builder.Container.GetType().AssemblyQualifiedName);
            }

            builder.Associate(WellKnownAssociations.Bindings, resultingBindings);
        }
    }
}