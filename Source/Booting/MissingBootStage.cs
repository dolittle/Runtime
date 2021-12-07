// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Exception that gets thrown when a <see cref="BootStage"/> is missing.
/// </summary>
public class MissingBootStage : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingBootStage"/> class.
    /// </summary>
    /// <param name="bootStage">The <see cref="BootStage"/> that is missing.</param>
    public MissingBootStage(BootStage bootStage)
        : base($"BootStage '{bootStage}' is missing - this could be due to a missing dependency that should be adding the boot stage")
    {
    }
}