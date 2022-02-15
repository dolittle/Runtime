// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Configuration;

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
        var loggerFactory = builder.GetAssociation(WellKnownAssociations.LoggerFactory) as ILoggerFactory;
        var logger = loggerFactory.CreateLogger<Configuration>();

        var configurationObjectProviders = new ConfigurationObjectProviders(typeFinder, builder.Container, logger);
        builder.Bindings.Bind<IConfigurationObjectProviders>().To(configurationObjectProviders);

        var configurationObjectTypes = typeFinder.FindMultiple<IConfigurationObject>();
        configurationObjectTypes.ForEach(_ =>
        {
            Log.BindConfigurationObject(logger, _.GetFriendlyConfigurationName(), _.AssemblyQualifiedName);
            _.ShouldBeImmutable();
            builder.Bindings.Bind(_).To(() =>
            {
                var instance = configurationObjectProviders.Provide(_);
                Log.ProvidingConfigurationObject(logger, _.GetFriendlyConfigurationName(), _.AssemblyQualifiedName, instance.GetHashCode());
                return instance;
            });
        });
    }
}
