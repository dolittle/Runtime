// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Types;

/// <summary>
/// Defines a system that is capable of finding types based on base types.
/// </summary>
public interface ITypeFinder
{
    /// <summary>
    /// Gets returns all collected types.
    /// </summary>
    IEnumerable<Type> All { get; }

    /// <summary>
    /// Find a single implementation of a base type.
    /// </summary>
    /// <typeparam name="T">Base type to find for.</typeparam>
    /// <returns>Type found.</returns>
    /// <remarks>
    /// If the base type is an interface, it will look for any types implementing the interface.
    /// If it is a class, it will find anyone inheriting from that class.
    /// </remarks>
    /// <exception cref="MultipleTypesFound">If there is more than one instance found.</exception>
    Type FindSingle<T>();

    /// <summary>
    /// Find multiple implementations of a base type.
    /// </summary>
    /// <typeparam name="T">Base type to find for.</typeparam>
    /// <returns>All types implementing or inheriting from the given base type.</returns>
    /// <remarks>
    /// If the base type is an interface, it will look for any types implementing the interface.
    /// If it is a class, it will find anyone inheriting from that class.
    /// </remarks>
    IEnumerable<Type> FindMultiple<T>();

    /// <summary>
    /// Find a single implementation of a base type.
    /// </summary>
    /// <param name="type">Base type to find for.</param>
    /// <returns>Type found.</returns>
    /// <remarks>
    /// If the base type is an interface, it will look for any types implementing the interface.
    /// If it is a class, it will find anyone inheriting from that class.
    /// </remarks>
    /// <exception cref="MultipleTypesFound">If there is more than one instance found.</exception>
    Type FindSingle(Type type);

    /// <summary>
    /// Find multiple implementations of a base type.
    /// </summary>
    /// <param name="type">Base type to find for.</param>
    /// <returns>All types implementing or inheriting from the given base type.</returns>
    /// <remarks>
    /// If the base type is an interface, it will look for any types implementing the interface.
    /// If it is a class, it will find anyone inheriting from that class.
    /// </remarks>
    IEnumerable<Type> FindMultiple(Type type);

    /// <summary>
    /// Find a single type using the full name, without assembly.
    /// </summary>
    /// <param name="fullName">full name of the type to find.</param>
    /// <returns>The type is found, null otherwise.</returns>
    Type FindTypeByFullName(string fullName);
}
