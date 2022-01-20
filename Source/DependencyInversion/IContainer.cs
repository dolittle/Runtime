// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a container for resolving types.
/// </summary>
public interface IContainer
{
    /// <summary>
    /// Get an instance of a specific type.
    /// </summary>
    /// <typeparam name="T">Type to get instance of.</typeparam>
    /// <returns>Instance of the type.</returns>
    T Get<T>();

    /// <summary>
    /// Get an instance of a specific type.
    /// </summary>
    /// <param name="type">Type to get instance of.</param>
    /// <returns>Instance of the type.</returns>
    object Get(Type type);
}