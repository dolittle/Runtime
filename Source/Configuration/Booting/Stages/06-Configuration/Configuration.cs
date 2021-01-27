// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Immutability;
using Microsoft.Extension.Logging;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Configuration
{
    /// <summary>
    /// Represents the system that manages the bindings for the IoC container for the
    /// <see cref="IConfigurationObject">configuration objects</see> in the system.
    /// </summary>
    public class Configuration : ICanPerformBootStage<NoSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.Configuration;

        /// <inheritdoc/>
        public void Perform(NoSettings settings, IBootStageBuilder builder)
        {
            var typeFinder = builder.GetAssociation(WellKnownAssociations.TypeFinder) as ITypeFinder;
            var loggerManager = builder.GetAssociation(WellKnownAssociations.LoggerManager) as ILoggerManager;
            var logger = loggerManager.CreateLogger<Configuration>();

            var configurationObjectProviders = new ConfigurationObjectProviders(typeFinder, builder.Container, logger);
            builder.Bindings.Bind<IConfigurationObjectProviders>().To(configurationObjectProviders);

            var configurationObjectTypes = typeFinder.FindMultiple<IConfigurationObject>();
            configurationObjectTypes.ForEach(_ =>
            {
                logger.Trace("Bind configuration object '{configurationObjectName} - {configurationObjectType}'", _.GetFriendlyConfigurationName(), _.AssemblyQualifiedName);
                _.ShouldBeImmutable();
                builder.Bindings.Bind(_).To(() =>
                {
                    var instance = configurationObjectProviders.Provide(_);
                    logger.Trace("Providing configuration object '{configurationObjectName} - {configurationTypeName}' - {configurationObjectHash}", _.GetFriendlyConfigurationName(), _.AssemblyQualifiedName, instance.GetHashCode());
                    return instance;
                });
            });
        }
    }
}