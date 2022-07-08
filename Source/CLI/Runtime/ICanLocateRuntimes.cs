// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// Defines a system that can locate Runtimes that are available to connect to.
/// </summary>
public interface ICanLocateRuntimes
{
    /// <summary>
    /// Gets the addresses of Runtimes that are available to connect to, or the address provided in the argument.
    /// </summary>
    /// <param name="argument">An optional address provided to the CLI as an argument.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of type <see cref="NamedRuntimeAddress"/> to available Runtimes.</returns>
    Task<IEnumerable<NamedRuntimeAddress>> GetAvailableRuntimeAddresses(MicroserviceAddress argument = null);
}
