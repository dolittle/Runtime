// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Exception that gets thrown when a <see cref="Type"/> does not implement the given generic interfaces.
/// </summary>
public class TypeDoesNotImplementGenericInterface : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDoesNotImplementGenericInterface"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="genericInterface">The open generic interface.</param>
    public TypeDoesNotImplementGenericInterface(Type type, Type genericInterface)
        : base($"{type} does not implement generic interface {genericInterface}")
    {
    }
}
