// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Exception that gets thrown when a type does not implement <see cref="ClientBase"/>.
/// </summary>
public class TypeDoesNotImplementClientBase : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDoesNotImplementClientBase"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> that does not implement.</param>
    public TypeDoesNotImplementClientBase(Type type)
        : base($"Type '{type.AssemblyQualifiedName}' does not implement expected ClientBase.")
    {
    }
}