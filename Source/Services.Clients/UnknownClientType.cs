// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Exception that gets thrown when a <see cref="ClientBase"/> is unknown and not registered.
/// </summary>
public class UnknownClientType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownClientType"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> of <see cref="ClientBase">client</see>.</param>
    public UnknownClientType(Type type)
        : base($"The client type '{type.AssemblyQualifiedName}' is not known. Make sure an implementation of '{nameof(IKnowAboutClients)}' return a definition of it.")
    {
    }
}