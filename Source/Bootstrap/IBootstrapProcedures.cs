// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Server.Bootstrap;

/// <summary>
/// Defines a system that knows about <see cref="ICanPerformBoostrapProcedure"/>.
/// </summary>
public interface IBootstrapProcedures
{
    /// <summary>
    /// Performs all bootstrap procedures.
    /// </summary>
    /// <returns></returns>
    Task PerformAll();
}
