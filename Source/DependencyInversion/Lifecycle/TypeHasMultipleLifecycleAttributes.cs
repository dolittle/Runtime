// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Lifecycle;

/// <summary>
/// Exception that gets thrown when a <see cref="Type"/> has multiple lifecycle attributes.
/// </summary>
public class TypeHasMultipleLifecycleAttributes : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeHasMultipleLifecycleAttributes"/> class.
    /// </summary>
    /// <param name="type">The type that has multiple lifecycle attributes.</param>
    public TypeHasMultipleLifecycleAttributes(Type type)
        : base($"The type ${type} has multiple lifecycle attributes, only one is allowed.")
    {
    }
}
