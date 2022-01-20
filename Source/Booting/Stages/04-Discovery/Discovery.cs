// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Assemblies.Rules;
using Dolittle.Runtime.Collections;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting.Stages;

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
        var entryAssembly = builder.GetAssociation<Assembly>(WellKnownAssociations.EntryAssembly);
        var loggerFactory = builder.GetAssociation<ILoggerFactory>(WellKnownAssociations.LoggerFactory);
        var logger = loggerFactory.CreateLogger<Discovery>();

        Log.Discovery(logger);
        var assemblies = Assemblies.Bootstrap.Boot.Start(logger, entryAssembly, settings.AssemblyProvider, _ =>
        {
            if (!(settings.IncludeAssembliesStartWith?.Count() > 0))
            {
                return;
            }
            settings.IncludeAssembliesStartWith.ForEach(name => Log.IncludingAssemblies(logger, name));
            _.ExceptAssembliesStartingWith(settings.IncludeAssembliesStartWith.ToArray());
        });
        Log.SetupTypeSystem(logger);
        var typeFinder = Types.Bootstrap.Boot.Start(assemblies, logger, entryAssembly);
        Log.TypeSystemReady(logger);

        builder.Bindings.Bind<IAssemblies>().To(assemblies);
        builder.Bindings.Bind<ITypeFinder>().To(typeFinder);

        builder.Associate(WellKnownAssociations.Assemblies, assemblies);
        builder.Associate(WellKnownAssociations.TypeFinder, typeFinder);
    }
}

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "  Type system ready")]
    internal static partial void TypeSystemReady(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Debug, "  Set up type system for discovery")]
    internal static partial void SetupTypeSystem(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Including assemblies starting with '{AssemblyName}'")]
    internal static partial void IncludingAssemblies(ILogger logger, string assemblyName);
    
    [LoggerMessage(0, LogLevel.Trace, "  Discovery")]
    internal static partial void Discovery(ILogger logger);
}
