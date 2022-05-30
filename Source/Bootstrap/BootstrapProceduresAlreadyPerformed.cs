// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Bootstrap;

/// <summary>
/// Exception that gets thrown when attempting to perform all bootstrap procedures more than once.
/// </summary>
public class BootstrapProceduresAlreadyPerformed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BootstrapProceduresAlreadyPerformed"/>.
    /// </summary>
    public BootstrapProceduresAlreadyPerformed()
        : base("Bootstrap procedures has already been performed")
    {
    }
}
