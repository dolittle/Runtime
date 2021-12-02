// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Defines a system that can provide assemblies.
/// </summary>
public interface ICanProvideAssemblies
{
    /// <summary>
    /// Gets the available assemblies that can be provided.
    /// </summary>
    /// <returns><see cref="IEnumerable{T}"/> of <see cref="Library"/>.</returns>
    IEnumerable<Library> Libraries { get; }

    /// <summary>
    /// Get a specific assembly based on a <see cref="Library"/> representation.
    /// </summary>
    /// <param name="library"><see cref="Library"/> representing the <see cref="Assembly"/>.</param>
    /// <returns>Loaded <see cref="Assembly"/>.</returns>
    Assembly GetFrom(Library library);
}