// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// Defines a system that can discover addresses if Runtimes that are available to connect to.
/// </summary>
public interface ICanDiscoverRuntimeAddresses
{
    /// <summary>
    /// Discovers addresses of available Runtimes.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of type <see cref="MicroserviceAddress"/> to available Runtimes.</returns>
    Task<IEnumerable<MicroserviceAddress>> Discover();
}