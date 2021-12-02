// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Protobuf;

/// <summary>
/// Exception that gets thrown when a <see cref="ServerCallContext"/> is missing a required arguments message in the header.
/// </summary>
public class MissingParserForMessageType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingParserForMessageType"/> class.
    /// </summary>
    /// <param name="type">The type of <see cref="IMessage"/> that does not have a parser.</param>
    public MissingParserForMessageType(Type type)
        : base($"Missing message parser for message type '{type.AssemblyQualifiedName}'")
    {
    }
}