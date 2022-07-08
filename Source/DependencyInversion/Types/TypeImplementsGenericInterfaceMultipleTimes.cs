// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Exception that gets thrown when a <see cref="Type"/> implements the given open generic interfaces multiple times.
/// </summary>
public class TypeImplementsGenericInterfaceMultipleTimes : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeImplementsGenericInterfaceMultipleTimes"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="genericInterface">The open generic interface.</param>
    public TypeImplementsGenericInterfaceMultipleTimes(Type type, Type genericInterface)
        : base($"{type} implements open generic interface {genericInterface} multiple times")
    {
    }
}
