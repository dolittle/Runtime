// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Booting.Stages;

/// <summary>
/// Represents the <see cref="BootStage.BootProcedures"/> stage of booting.
/// </summary>
public class BootProcedures : ICanPerformBootStage<BootProceduresSettings>
{
    /// <inheritdoc/>
    public BootStage BootStage => BootStage.BootProcedures;

    /// <inheritdoc/>
    public void Perform(BootProceduresSettings settings, IBootStageBuilder builder)
    {
        if (settings.Enabled)
        {
            builder.Container.Get<IBootProcedures>().Perform();
        }
    }
}