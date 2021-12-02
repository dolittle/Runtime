// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Configuration.Files.Booting.Stages;

/// <summary>
/// Represents bindings for booting.
/// </summary>
public class PreConfiguration : ICanRunBeforeBootStage<NoSettings>
{
    /// <inheritdoc/>
    public BootStage BootStage => BootStage.Configuration;

    /// <inheritdoc/>
    public void Perform(NoSettings settings, IBootStageBuilder builder)
    {
        var typeFinder = builder.GetAssociation(WellKnownAssociations.TypeFinder) as ITypeFinder;
        var parsers = new ConfigurationFileParsers(typeFinder, builder.Container);
        builder.Bindings.Bind<IConfigurationFileParsers>().To(parsers);
    }
}