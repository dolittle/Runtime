// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Autofac;

/// <summary>
/// Exception that gets thrown when there is more than one constructor and activation can't be resolved.
/// </summary>
public class AmbiguousConstructor : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbiguousConstructor"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> that has more than one constructor.</param>
    public AmbiguousConstructor(Type type)
        : base($"Type '{type.AssemblyQualifiedName} has more than one constructor and its not possible to resolve which to use.'")
    {
    }
}