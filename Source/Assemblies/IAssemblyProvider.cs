// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Defines a system that can provide <see cref="Assembly">assemblies</see>.
/// </summary>
public interface IAssemblyProvider
{
    /// <summary>
    /// Get all the <see cref="Assembly">assemblies</see> that can be provided.
    /// </summary>
    /// <returns><see cref="IEnumerable{Assembly}">Assemblies</see> provided.</returns>
    IEnumerable<Assembly> GetAll();
}