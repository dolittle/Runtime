// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Defines a system that can deal with all the <see cref="BootStage">boot stages</see>.
/// </summary>
public interface IBootStages
{
    /// <summary>
    /// Perform all boot stages.
    /// </summary>
    /// <param name="boot"><see cref="Boot"/> details.</param>
    /// <returns><see cref="BootStagesResult"/>.</returns>
    BootStagesResult Perform(Boot boot);
}