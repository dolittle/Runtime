// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Serialization.Json;

/// <summary>
/// Exception that gets thrown when the serializer is unable to instantiate a type.
/// Typically due to the lack of a default constructor and mismatched parameters in the non-default constructor(s).
/// </summary>
public class UnableToInstantiateInstanceOfType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnableToInstantiateInstanceOfType"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> that was not possible to instantiate.</param>
    public UnableToInstantiateInstanceOfType(Type type)
        : base($"Cannot instantiate type {type?.FullName ?? "[NULL]"}.  Ensure that the type has a default constructor or that there is a matching constructor with the ctor parameters matching the property names.")
    {
    }
}