// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.DependencyInversion.Booting;

/// <summary>
/// Exception that gets thrown when trying to resolve a type that is not bound in the container.
/// </summary>
public class TypeNotBoundInContainer : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeNotBoundInContainer"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> that was resolved from the container.</param>
    /// <param name="supportedDependencies"><see cref="IEnumerable{T}">Collection</see> of <see cref="Type">supported types</see>.</param>
    public TypeNotBoundInContainer(Type type, IEnumerable<Type> supportedDependencies)
        : base($"The type '{type.AssemblyQualifiedName}' is not bound in the container.\nSupported dependencies are:\n\n{string.Join("\n", supportedDependencies)}\n\n")
    {
    }
}