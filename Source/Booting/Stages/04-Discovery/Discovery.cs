// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Assemblies.Rules;
using Dolittle.Runtime.Collections;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Scheduling;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents the <see cref="BootStage.Discovery"/> stage of booting.
    /// </summary>
    public class Discovery : ICanPerformBootStage<DiscoverySettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.Discovery;

        /// <inheritdoc/>
        public void Perform(DiscoverySettings settings, IBootStageBuilder builder)
        {
            var entryAssembly = builder.GetAssociation(WellKnownAssociations.EntryAssembly) as Assembly;
            var loggerManager = builder.GetAssociation(WellKnownAssociations.LoggerManager) as ILoggerManager;
            var logger = loggerManager.CreateLogger<Discovery>();
            var scheduler = builder.GetAssociation(WellKnownAssociations.Scheduler) as IScheduler;

            logger.LogDebug("  Discovery");
            var assemblies = Assemblies.Bootstrap.Boot.Start(logger, entryAssembly, settings.AssemblyProvider, _ =>
            {
                if (settings.IncludeAssembliesStartWith?.Count() > 0)
                {
                    settings.IncludeAssembliesStartWith.ForEach(name => logger.LogTrace("Including assemblies starting with '{name}'", name));
                    _.ExceptAssembliesStartingWith(settings.IncludeAssembliesStartWith.ToArray());
                }
            });
            logger.LogDebug("  Set up type system for discovery");
            var typeFinder = Types.Bootstrap.Boot.Start(assemblies, scheduler, logger, entryAssembly);
            logger.LogDebug("  Type system ready");

            builder.Bindings.Bind<IAssemblies>().To(assemblies);
            builder.Bindings.Bind<ITypeFinder>().To(typeFinder);

            builder.Associate(WellKnownAssociations.Assemblies, assemblies);
            builder.Associate(WellKnownAssociations.TypeFinder, typeFinder);
        }
    }
}